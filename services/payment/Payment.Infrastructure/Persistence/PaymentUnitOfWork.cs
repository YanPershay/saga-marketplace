using System.Text.Json;
using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.Events;
using Microsoft.EntityFrameworkCore;
using Payment.Application.Abstractions;
using Payment.Infrastructure.Persistence.Inbox;
using Payment.Infrastructure.Persistence.Outbox;

namespace Payment.Infrastructure.Persistence;

public sealed class PaymentUnitOfWork : IPaymentUnitOfWork
{
    private const string PaymentSucceededRoutingKey = "payment.succeeded";
    private const string PaymentFailedRoutingKey = "payment.failed";

    private readonly PaymentDbContext _context;
    
    public PaymentUnitOfWork(PaymentDbContext context)
    {
        _context = context;
    }

    public async Task<bool> HasInboxMessageAsync(
        Guid messageId, 
        string consumerName, 
        CancellationToken cancellationToken = default)
    {
        return await _context.InboxMessages
            .AnyAsync(m => m.MessageId == messageId
                           && m.ConsumerName == consumerName);
    }

    public async Task ProcessPaymentAndSaveOutboxAsync(
        EventEnvelope<InventoryReservedIntegrationEvent> envelope,
        string consumerName,
        bool forceFailure,
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

        var payment = Domain.Payments.Payment.Create(
            envelope.Payload.OrderId,
            amount: 100m);

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

        if (forceFailure)
        {
            payment.MarkAsFailed();

            var resultEvent = new PaymentFailedIntegrationEvent(
                payment.OrderId,
                payment.Id,
                payment.Amount,
                "Payment simulation forced failure",
                DateTimeOffset.UtcNow);

            var resultEnvelope = EventEnvelope<PaymentFailedIntegrationEvent>.Create(
                resultEvent,
                envelope.CorrelationId,
                envelope.MessageId);

            resultMessageId = resultEnvelope.MessageId;
            resultCorrelationId = resultEnvelope.CorrelationId;
            resultEventType = resultEnvelope.EventType;
            resultOccurredAtUtc = resultEnvelope.OccurredAtUtc;
            routingKey = PaymentFailedRoutingKey;
            payload = JsonSerializer.Serialize(resultEnvelope, jsonOptions);
        }
        else
        {
            payment.MarkAsSucceeded();
            
            var resultEvent = new PaymentSucceededIntegrationEvent(
                payment.OrderId,
                payment.Id,
                payment.Amount,
                DateTimeOffset.UtcNow);

            var resultEnvelope = EventEnvelope<PaymentSucceededIntegrationEvent>.Create(
                resultEvent,
                envelope.CorrelationId,
                envelope.MessageId);

            resultMessageId = resultEnvelope.MessageId;
            resultCorrelationId = resultEnvelope.CorrelationId;
            resultEventType = resultEnvelope.EventType;
            resultOccurredAtUtc = resultEnvelope.OccurredAtUtc;
            routingKey = PaymentSucceededRoutingKey;
            payload = JsonSerializer.Serialize(resultEnvelope, jsonOptions);
        }

        var inboxMessage = InboxMessage.Create(
            envelope.MessageId,
            envelope.EventType,
            consumerName);

        var outboxMessage = OutboxMessage.Create(
            resultMessageId,
            resultCorrelationId,
            resultEventType,
            routingKey,
            payload,
            resultOccurredAtUtc);
        
        await _context.Payments.AddAsync(payment, cancellationToken);
        await _context.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
        await _context.InboxMessages.AddAsync(inboxMessage, cancellationToken);
        
        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}