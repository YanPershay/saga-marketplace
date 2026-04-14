namespace BuildingBlocks.Messaging;

public sealed class EventEnvelope<TPayload> where TPayload : IIntegrationEvent
{
    public Guid MessageId { get; }
    public Guid CorrelationId { get; }
    public Guid? CausationId { get; }
    public string EventType { get; }
    public int Version { get; }
    public DateTime OccurredAtUtc { get; }
    public TPayload Payload { get; }

    public EventEnvelope(
        Guid messageId,
        Guid correlationId,
        string eventType,
        int version,
        DateTime occurredAtUtc,
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
}