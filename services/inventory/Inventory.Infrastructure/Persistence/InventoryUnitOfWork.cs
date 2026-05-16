using System.Text.Json;
using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.Events;
using Inventory.Application.Abstractions;
using Inventory.Infrastructure.Persistence.Inbox;
using Inventory.Infrastructure.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence;

public sealed class InventoryUnitOfWork : IInventoryUnitOfWork
{
    private const string InventoryReservedRoutingKey = "inventory.reserved";
    private const string InventoryReservationFailedRoutingKey = "inventory.reservation.failed";

    private readonly InventoryDbContext _context;

    public InventoryUnitOfWork(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<bool> HasInboxMessageAsync(
        Guid messageId,
        string consumerName,
        CancellationToken cancellationToken = default)
    {
        return await _context.InboxMessages
            .AnyAsync(
                message => message.MessageId == messageId &&
                           message.ConsumerName == consumerName,
                cancellationToken);
    }

    public async Task ReserveInventoryAndSaveOutboxAsync(
        EventEnvelope<OrderCreatedIntegrationEvent> envelope,
        string consumerName,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await _context.Database
            .BeginTransactionAsync(cancellationToken);

        var messageAlreadyProcessed = await HasInboxMessageAsync(
            envelope.MessageId,
            consumerName,
            cancellationToken);

        if (messageAlreadyProcessed)
            return;

        var orderCreated = envelope.Payload;

        var requestedProductIds = orderCreated.Items
            .Select(item => item.ProductId)
            .Distinct()
            .ToList();

        var stockItems = await _context.StockItems
            .Where(stockItem => requestedProductIds.Contains(stockItem.ProductId))
            .ToListAsync(cancellationToken);

        var stockByProductId = stockItems.ToDictionary(stockItem => stockItem.ProductId);

        var failedItems = new List<InventoryReservationFailedItem>();

        foreach (var orderItem in orderCreated.Items)
        {
            var availableQuantity = stockByProductId.TryGetValue(orderItem.ProductId, out var stockItem)
                ? stockItem.QuantityAvailable
                : 0;

            if (availableQuantity < orderItem.Quantity)
            {
                failedItems.Add(
                    new InventoryReservationFailedItem(
                        orderItem.ProductId,
                        orderItem.Quantity,
                        availableQuantity));
            }
        }

        Guid resultMessageId;
        Guid resultCorrelationId;
        string resultEventType;
        DateTimeOffset resultOccurredAtUtc;
        string routingKey;
        string payload;

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        if (failedItems.Count > 0)
        {
            var resultEvent = new InventoryReservationFailedIntegrationEvent(
                orderCreated.OrderId,
                "Insufficient stock",
                failedItems);

            var resultEnvelope = EventEnvelope<InventoryReservationFailedIntegrationEvent>.Create(
                resultEvent,
                envelope.CorrelationId,
                envelope.MessageId);

            resultMessageId = resultEnvelope.MessageId;
            resultCorrelationId = resultEnvelope.CorrelationId;
            resultEventType = resultEnvelope.EventType;
            resultOccurredAtUtc = resultEnvelope.OccurredAtUtc;
            routingKey = InventoryReservationFailedRoutingKey;
            payload = JsonSerializer.Serialize(resultEnvelope, jsonOptions);
        }
        else
        {
            foreach (var orderItem in orderCreated.Items)
            {
                var stockItem = stockByProductId[orderItem.ProductId];
                stockItem.Reserve(orderItem.Quantity);
            }

            var resultEvent = new InventoryReservedIntegrationEvent(
                orderCreated.OrderId,
                orderCreated.Items
                    .Select(item => new InventoryReservedItem(
                        item.ProductId,
                        item.Quantity))
                    .ToList());

            var resultEnvelope = EventEnvelope<InventoryReservedIntegrationEvent>.Create(
                resultEvent,
                envelope.CorrelationId,
                envelope.MessageId);

            resultMessageId = resultEnvelope.MessageId;
            resultCorrelationId = resultEnvelope.CorrelationId;
            resultEventType = resultEnvelope.EventType;
            resultOccurredAtUtc = resultEnvelope.OccurredAtUtc;
            routingKey = InventoryReservedRoutingKey;
            payload = JsonSerializer.Serialize(resultEnvelope, jsonOptions);
        }

        var inboxMessage = InboxMessage.Create(
            envelope.MessageId, envelope.EventType, consumerName);

        var outboxMessage = OutboxMessage.Create(
            resultMessageId,
            resultCorrelationId,
            resultEventType,
            routingKey,
            payload,
            resultOccurredAtUtc);
        
        await _context.InboxMessages.AddAsync(inboxMessage, cancellationToken);
        await _context.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
        
        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
    
    public async Task CommitReservationAndSaveInboxAsync(
        EventEnvelope<InventoryCommitRequestedIntegrationEvent> envelope,
        string consumerName,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await _context.Database
            .BeginTransactionAsync(cancellationToken);

        var messageAlreadyProcessed = await HasInboxMessageAsync(
            envelope.MessageId,
            consumerName,
            cancellationToken);

        if (messageAlreadyProcessed)
            return;

        var commitRequested = envelope.Payload;

        var productIds = commitRequested.Items
            .Select(item => item.ProductId)
            .Distinct()
            .ToList();

        var stockItems = await _context.StockItems
            .Where(stockItem => productIds.Contains(stockItem.ProductId))
            .ToListAsync(cancellationToken);

        var stockByProductId = stockItems.ToDictionary(stockItem => stockItem.ProductId);

        foreach (var item in commitRequested.Items)
        {
            if (!stockByProductId.TryGetValue(item.ProductId, out var stockItem))
                throw new InvalidOperationException(
                    $"Stock item {item.ProductId} was not found.");

            stockItem.CommitReservation(item.Quantity);
        }

        var inboxMessage = InboxMessage.Create(
            envelope.MessageId,
            envelope.EventType,
            consumerName);

        await _context.InboxMessages.AddAsync(inboxMessage, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

}