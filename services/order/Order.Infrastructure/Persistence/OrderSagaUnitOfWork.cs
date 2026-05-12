using System.Text.Json;
using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.Events;
using Microsoft.EntityFrameworkCore;
using Order.Application.Abstractions;
using Order.Infrastructure.Persistence.Inbox;
using Order.Infrastructure.Persistence.Outbox;
using Microsoft.Extensions.Logging;

namespace Order.Infrastructure.Persistence;

public sealed class OrderSagaUnitOfWork : IOrderSagaUnitOfWork
{
    private const string PaymentRequestedRoutingKey = "payment.requested";
    private const string InventoryReleaseRequestedRoutingKey = "inventory.release.requested";
    private const string ShipmentRequestedRoutingKey = "shipment.requested";

    private const string PaymentRefundRequestedRoutingKey = "payment.refund.requested";

    private readonly OrderDbContext _context;
    private readonly ILogger<OrderSagaUnitOfWork> _logger;

    public OrderSagaUnitOfWork(
        OrderDbContext context,
        ILogger<OrderSagaUnitOfWork> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> HasInboxMessageAsync(
        Guid messageId,
        string consumerName,
        CancellationToken cancellationToken = default)
    {
        return await _context.InboxMessages
            .AnyAsync(m =>
                    m.MessageId == messageId
                    && m.ConsumerName == consumerName,
                cancellationToken);
    }

    public async Task HandleInventoryReservedAsync(
        EventEnvelope<InventoryReservedIntegrationEvent> envelope,
        string consumerName,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await _context.Database
            .BeginTransactionAsync(cancellationToken);

        var alreadyProcessed = await HasInboxMessageAsync(
            envelope.MessageId,
            consumerName,
            cancellationToken);

        if (alreadyProcessed)
            return;

        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == envelope.Payload.OrderId,
                cancellationToken);

        if (order is null)
            throw new InvalidOperationException(
                $"Order {envelope.Payload.OrderId} was not found.");

        var inboxMessage = InboxMessage.Create(
            envelope.MessageId,
            envelope.EventType,
            consumerName);

        await _context.InboxMessages.AddAsync(inboxMessage, cancellationToken);

        if (order.Status != Domain.Orders.OrderStatus.AwaitingInventory)
        {
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return;
        }

        order.MarkInventoryReserved();
        order.MarkAwaitingPayment();
        _logger.LogInformation(
            "Order {OrderId} moved to {OrderStatus}. Creating {EventType} outbox message.",
            order.Id,
            order.Status,
            nameof(PaymentRequestedIntegrationEvent));

        var paymentRequestedEvent = new PaymentRequestedIntegrationEvent(
            order.Id,
            order.TotalPrice);

        var paymentRequestedEnvelope = EventEnvelope<PaymentRequestedIntegrationEvent>.Create(
            paymentRequestedEvent,
            envelope.CorrelationId,
            envelope.MessageId);

        var payload = JsonSerializer.Serialize(paymentRequestedEnvelope,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });

        var outboxMessage = OutboxMessage.Create(
            paymentRequestedEnvelope.MessageId,
            paymentRequestedEnvelope.CorrelationId,
            paymentRequestedEnvelope.EventType,
            PaymentRequestedRoutingKey,
            payload,
            paymentRequestedEnvelope.OccurredAtUtc);

        await _context.OutboxMessages.AddAsync(outboxMessage, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task HandleInventoryReservationFailedAsync(
        EventEnvelope<InventoryReservationFailedIntegrationEvent> envelope,
        string consumerName,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await _context.Database
            .BeginTransactionAsync(cancellationToken);

        var alreadyProcessed = await HasInboxMessageAsync(
            envelope.MessageId,
            consumerName,
            cancellationToken);

        if (alreadyProcessed)
            return;

        var order = await _context.Orders
            .FirstOrDefaultAsync(
                o => o.Id == envelope.Payload.OrderId,
                cancellationToken);

        if (order is null)
            throw new InvalidOperationException(
                $"Order {envelope.Payload.OrderId} was not found.");

        var inboxMessage = InboxMessage.Create(
            envelope.MessageId,
            envelope.EventType,
            consumerName);

        await _context.InboxMessages.AddAsync(inboxMessage, cancellationToken);

        if (order.Status is Domain.Orders.OrderStatus.Cancelled or Domain.Orders.OrderStatus.Completed)
        {
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return;
        }

        order.Cancel();
        _logger.LogInformation(
            "Order {OrderId} cancelled because inventory reservation failed. Reason: {Reason}.",
            order.Id,
            envelope.Payload.Reason);

        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task HandlePaymentSucceededAsync(
        EventEnvelope<PaymentSucceededIntegrationEvent> envelope,
        string consumerName,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await _context.Database
            .BeginTransactionAsync(cancellationToken);

        var alreadyProcessed = await HasInboxMessageAsync(
            envelope.MessageId,
            consumerName,
            cancellationToken);

        if (alreadyProcessed)
            return;

        var order = await _context.Orders
            .FirstOrDefaultAsync(
                o => o.Id == envelope.Payload.OrderId,
                cancellationToken);

        if (order is null)
            throw new InvalidOperationException(
                $"Order {envelope.Payload.OrderId} was not found.");

        var inboxMessage = InboxMessage.Create(
            envelope.MessageId,
            envelope.EventType,
            consumerName);

        await _context.InboxMessages.AddAsync(inboxMessage, cancellationToken);

        if (order.Status != Domain.Orders.OrderStatus.AwaitingPayment)
        {
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return;
        }

        order.MarkPaymentSucceeded();
        order.MarkAwaitingShipment();
        _logger.LogInformation(
            "Order {OrderId} moved to {OrderStatus}. Creating {EventType} outbox message.",
            order.Id,
            order.Status,
            nameof(ShipmentRequestedIntegrationEvent));

        var shipmentRequestedEvent = new ShipmentRequestedIntegrationEvent(order.Id);

        var shipmentRequestedEnvelope = EventEnvelope<ShipmentRequestedIntegrationEvent>
            .Create(
                shipmentRequestedEvent,
                envelope.CorrelationId,
                envelope.MessageId);

        var payload = JsonSerializer.Serialize(shipmentRequestedEnvelope, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var outboxMessage = OutboxMessage.Create(
            shipmentRequestedEnvelope.MessageId,
            shipmentRequestedEnvelope.CorrelationId,
            shipmentRequestedEnvelope.EventType,
            ShipmentRequestedRoutingKey,
            payload,
            shipmentRequestedEnvelope.OccurredAtUtc);

        await _context.OutboxMessages.AddAsync(outboxMessage, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task HandlePaymentFailedAsync(
        EventEnvelope<PaymentFailedIntegrationEvent> envelope,
        string consumerName,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await _context.Database
            .BeginTransactionAsync(cancellationToken);

        var alreadyProcessed = await HasInboxMessageAsync(
            envelope.MessageId,
            consumerName,
            cancellationToken);

        if (alreadyProcessed)
            return;

        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(
                o => o.Id == envelope.Payload.OrderId,
                cancellationToken);

        if (order is null)
            throw new InvalidOperationException(
                $"Order {envelope.Payload.OrderId} was not found.");

        var inboxMessage = InboxMessage.Create(
            envelope.MessageId,
            envelope.EventType,
            consumerName);

        await _context.InboxMessages.AddAsync(inboxMessage, cancellationToken);

        if (order.Status != Domain.Orders.OrderStatus.AwaitingPayment)
        {
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return;
        }

        order.MarkPaymentFailed();
        order.Cancel();
        _logger.LogInformation(
            "Order {OrderId} cancelled because payment failed. Creating {EventType} compensation event.",
            order.Id,
            nameof(InventoryReleaseRequestedIntegrationEvent));

        var releaseEvent = new InventoryReleaseRequestedIntegrationEvent(
            order.Id,
            "Payment failed",
            order.OrderItems
                .Select(item => new InventoryReleaseRequestedItem(
                    item.ProductId,
                    item.Quantity))
                .ToList());

        var releaseEnvelope = EventEnvelope<InventoryReleaseRequestedIntegrationEvent>.Create(
            releaseEvent,
            envelope.CorrelationId,
            envelope.MessageId);

        var payload = JsonSerializer.Serialize(releaseEnvelope, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var outboxMessage = OutboxMessage.Create(
            releaseEnvelope.MessageId,
            releaseEnvelope.CorrelationId,
            releaseEnvelope.EventType,
            InventoryReleaseRequestedRoutingKey,
            payload,
            releaseEnvelope.OccurredAtUtc);

        await _context.OutboxMessages.AddAsync(outboxMessage, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task HandleShipmentCreatedAsync(EventEnvelope<ShipmentCreatedIntegrationEvent> envelope,
        string consumerName,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await _context.Database
            .BeginTransactionAsync(cancellationToken);

        var alreadyProcessed = await HasInboxMessageAsync(
            envelope.MessageId,
            consumerName,
            cancellationToken);

        if (alreadyProcessed)
            return;

        var order = await _context.Orders
            .FirstOrDefaultAsync(
                o => o.Id == envelope.Payload.OrderId,
                cancellationToken);

        if (order is null)
            throw new InvalidOperationException(
                $"Order {envelope.Payload.OrderId} was not found.");

        var inboxMessage = InboxMessage.Create(
            envelope.MessageId,
            envelope.EventType,
            consumerName);

        await _context.InboxMessages.AddAsync(inboxMessage, cancellationToken);

        if (order.Status != Domain.Orders.OrderStatus.AwaitingShipment)
        {
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return;
        }

        order.MarkShipmentCreated();
        order.Complete();
        _logger.LogInformation(
            "Order {OrderId} completed after shipment was created.",
            order.Id);

        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task HandleShipmentFailedAsync(
        EventEnvelope<ShipmentFailedIntegrationEvent> envelope,
        string consumerName,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await _context.Database
            .BeginTransactionAsync(cancellationToken);
        
        var alreadyProcessed = await HasInboxMessageAsync(
            envelope.MessageId,
            consumerName,
            cancellationToken);

        if (alreadyProcessed)
            return;
        
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(
                o => o.Id == envelope.Payload.OrderId,
                cancellationToken);

        if (order is null)
            throw new InvalidOperationException(
                $"Order {envelope.Payload.OrderId} was not found.");
        
        var inboxMessage = InboxMessage.Create(
            envelope.MessageId,
            envelope.EventType,
            consumerName);
        
        await _context.InboxMessages.AddAsync(inboxMessage, cancellationToken);

        if (order.Status != Domain.Orders.OrderStatus.AwaitingShipment)
        {
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return;
        }
        
        order.MarkShipmentFailed();
        order.Cancel();
        _logger.LogInformation(
            "Order {OrderId} cancelled because shipment failed. Creating compensation events: {PaymentRefundEventType}, {InventoryReleaseEventType}.",
            order.Id,
            nameof(PaymentRefundRequestedIntegrationEvent),
            nameof(InventoryReleaseRequestedIntegrationEvent));

        var paymentRefundRequestedEvent = new PaymentRefundRequestedIntegrationEvent(
            order.Id,
            "Shipment failed");

        var paymentRefundRequestedEnvelope = EventEnvelope<PaymentRefundRequestedIntegrationEvent>.Create(
            paymentRefundRequestedEvent,
            envelope.CorrelationId,
            envelope.MessageId);

        var paymentRefundPayload = JsonSerializer.Serialize(paymentRefundRequestedEnvelope, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var paymentRefundOutboxMessage = OutboxMessage.Create(
            paymentRefundRequestedEnvelope.MessageId,
            paymentRefundRequestedEnvelope.CorrelationId,
            paymentRefundRequestedEnvelope.EventType,
            PaymentRefundRequestedRoutingKey,
            paymentRefundPayload,
            paymentRefundRequestedEnvelope.OccurredAtUtc);

        var inventoryReleaseRequestedEvent = new InventoryReleaseRequestedIntegrationEvent(
            order.Id,
            "Shipment failed",
            order.OrderItems
                .Select(item => new InventoryReleaseRequestedItem(
                    item.ProductId,
                    item.Quantity))
                .ToList());

        var inventoryReleaseRequestedEnvelope = EventEnvelope<InventoryReleaseRequestedIntegrationEvent>.Create(
            inventoryReleaseRequestedEvent,
            envelope.CorrelationId,
            envelope.MessageId);

        var inventoryReleasePayload = JsonSerializer.Serialize(inventoryReleaseRequestedEnvelope, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var inventoryReleaseOutboxMessage = OutboxMessage.Create(
            inventoryReleaseRequestedEnvelope.MessageId,
            inventoryReleaseRequestedEnvelope.CorrelationId,
            inventoryReleaseRequestedEnvelope.EventType,
            InventoryReleaseRequestedRoutingKey,
            inventoryReleasePayload,
            inventoryReleaseRequestedEnvelope.OccurredAtUtc);

        await _context.OutboxMessages.AddAsync(paymentRefundOutboxMessage, cancellationToken);
        await _context.OutboxMessages.AddAsync(inventoryReleaseOutboxMessage, cancellationToken);
        
        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}