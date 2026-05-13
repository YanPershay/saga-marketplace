using System.Diagnostics;

namespace BuildingBlocks.Messaging;

public sealed class EventEnvelope<TPayload> where TPayload : IIntegrationEvent
{
    public Guid MessageId { get; }
    public Guid CorrelationId { get; }
    public Guid? CausationId { get; }
    public string EventType { get; }
    public int Version { get; }
    public DateTimeOffset OccurredAtUtc { get; }
    public TPayload Payload { get; }
    public string? TraceParent { get; init; }
    public string? TraceState { get; init; }

    public EventEnvelope(
        Guid messageId,
        Guid correlationId,
        string eventType,
        int version,
        DateTimeOffset occurredAtUtc,
        TPayload payload,
        Guid? causationId = null)
    {
        if (messageId == Guid.Empty)
            throw new ArgumentException("MessageId cannot be empty.", nameof(messageId));

        if (correlationId == Guid.Empty)
            throw new ArgumentException("CorrelationId cannot be empty.", nameof(correlationId));

        ArgumentException.ThrowIfNullOrEmpty(eventType, nameof(eventType));
        ArgumentNullException.ThrowIfNull(payload, nameof(payload));
        
        if (version <= 0)
            throw new ArgumentOutOfRangeException(nameof(version), "Version must be greater than zero.");

        MessageId = messageId;
        CorrelationId = correlationId;
        CausationId = causationId;
        EventType = eventType;
        Version = version;
        OccurredAtUtc = occurredAtUtc;
        Payload = payload;
    }
    
    public static EventEnvelope<TPayload> Create(
        TPayload payload,
        Guid? correlationId = null,
        Guid? causationId = null)
    {
        var messageId = Guid.NewGuid();
        var corrId = correlationId ?? Guid.NewGuid();


        return new EventEnvelope<TPayload>(
            messageId: messageId,
            correlationId: corrId,
            eventType: typeof(TPayload).Name,
            version: payload.Version,
            occurredAtUtc: DateTimeOffset.UtcNow,
            payload: payload,
            causationId: causationId)
        {
            TraceParent = Activity.Current?.Id,
            TraceState = Activity.Current?.TraceStateString
        };
    }
}