using System.Text.Json;
using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.Events;
using Microsoft.EntityFrameworkCore;
using Order.Application.Abstractions;
using Order.Infrastructure.Persistence.Inbox;
using Order.Infrastructure.Persistence.Outbox;

namespace Order.Infrastructure.Persistence;

public sealed class OrderSagaUnitOfWork : IOrderSagaUnitOfWork
{
    private const string PaymentRequestedRoutingKey = "payment.requested";
    private const string InventoryReleaseRequestedRoutingKey = "inventory.release.requested";

    private readonly OrderDbContext _context;

    public OrderSagaUnitOfWork(OrderDbContext context)
    {
        _context = context;
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
}