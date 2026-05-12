using System.Text.Json;
using Shipping.Infrastructure.Persistence.Outbox;
using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.Events;
using Microsoft.EntityFrameworkCore;
using Shipping.Application.Abstractions;
using Shipping.Domain.Shipments;
using Shipping.Infrastructure.Persistence.Inbox;

namespace Shipping.Infrastructure.Persistence;

public sealed class ShippingUnitOfWork : IShippingUnitOfWork
{
    private const string ShipmentCreatedRoutingKey = "shipment.created";
    private const string ShipmentFailedRoutingKey = "shipment.failed";
    
    private readonly ShippingDbContext _context;

    public ShippingUnitOfWork(ShippingDbContext context)
    {
        _context = context;
    }

    public async Task<bool> HasInboxMessageAsync(Guid messageId, string consumerName, CancellationToken cancellationToken = default)
    {
        return await _context.InboxMessages
            .AnyAsync(m => m.MessageId == messageId
                           && m.ConsumerName == consumerName,
                      cancellationToken);
    }

    public async Task CreateShipmentAndSaveOutboxAsync(
        EventEnvelope<ShipmentRequestedIntegrationEvent> envelope,
        string consumerName,
        bool forceFailure,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        
        var alreadyProcessed = await HasInboxMessageAsync(
            envelope.MessageId,
            consumerName,
            cancellationToken);

        if (alreadyProcessed)
            return;
        
        var shipment = Shipment.Create(
            envelope.Payload.OrderId);
        
        if (forceFailure)
            shipment.MarkFailed();
        else
            shipment.MarkCreated();
        
        await _context.Shipments.AddAsync(shipment, cancellationToken);
        
        var inboxMessage = InboxMessage.Create(
            envelope.MessageId,
            envelope.EventType,
            consumerName);
        
        await _context.InboxMessages.AddAsync(inboxMessage, cancellationToken);

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
            var integrationEvent = new ShipmentFailedIntegrationEvent(
                shipment.OrderId,
                "Shipping simulation failure",
                DateTimeOffset.UtcNow);

            var eventEnvelope = EventEnvelope<ShipmentFailedIntegrationEvent>.Create(
                integrationEvent,
                envelope.CorrelationId,
                envelope.MessageId);

            resultMessageId = eventEnvelope.MessageId;
            resultCorrelationId = eventEnvelope.CorrelationId;
            resultEventType = eventEnvelope.EventType;
            resultOccurredAtUtc = eventEnvelope.OccurredAtUtc;
            routingKey = ShipmentFailedRoutingKey;
            payload = JsonSerializer.Serialize(eventEnvelope, jsonOptions);
        }
        else
        {
            var integrationEvent = new ShipmentCreatedIntegrationEvent(
                shipment.OrderId,
                shipment.Id,
                DateTimeOffset.UtcNow);

            var eventEnvelope = EventEnvelope<ShipmentCreatedIntegrationEvent>.Create(
                integrationEvent,
                envelope.CorrelationId,
                envelope.MessageId);

            resultMessageId = eventEnvelope.MessageId;
            resultCorrelationId = eventEnvelope.CorrelationId;
            resultEventType = eventEnvelope.EventType;
            resultOccurredAtUtc = eventEnvelope.OccurredAtUtc;
            routingKey = ShipmentCreatedRoutingKey;
            payload = JsonSerializer.Serialize(eventEnvelope, jsonOptions);
        }

        var outboxMessage = OutboxMessage.Create(
            resultMessageId,
            resultCorrelationId,
            resultEventType,
            routingKey,
            payload,
            resultOccurredAtUtc);

        await _context.OutboxMessages.AddAsync(outboxMessage, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}