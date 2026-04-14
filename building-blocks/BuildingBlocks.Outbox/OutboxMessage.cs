namespace BuildingBlocks.Outbox;

public class OutboxMessage
{
    public Guid Id { get; private set; }
    public Guid MessageId { get; }
    public Guid CorrelationId { get; }
    public Guid? CausationId { get; }
    public string EventType { get; }
    public int Version { get; }
    public DateTime OccurredAtUtc { get; }
    public string Payload { get; }
    
    public OutboxStatus Status { get; private set; }
    public int RetryCount { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; }
    public DateTimeOffset? ProcessedAtUtc { get; private set; }
    public string? Error { get; private set; }

    public OutboxMessage(
        Guid messageId,
        Guid correlationId,
        Guid? causationId,
        string eventType,
        int version,
        DateTime occurredAtUtc,
        string payload)
    {
        if (messageId == Guid.Empty)
            throw new ArgumentException("MessageId cannot be empty.", nameof(messageId));
        
        if (correlationId == Guid.Empty)
            throw new ArgumentException("CorrelationId cannot be empty.", nameof(correlationId));
        
        ArgumentException.ThrowIfNullOrEmpty(eventType, nameof(eventType));
        ArgumentException.ThrowIfNullOrEmpty(payload, nameof(payload));
        
        if (version <= 0)
            throw new ArgumentOutOfRangeException(nameof(version), "Version must be greater than zero.");

        Id = Guid.NewGuid();
        MessageId = messageId;
        CorrelationId = correlationId;
        CausationId = causationId;
        EventType = eventType;
        Version = version;
        OccurredAtUtc = occurredAtUtc;
        Payload = payload;
        Status = OutboxStatus.Pending;
        CreatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void IncrementRetry()
    {
        if (Status == OutboxStatus.Sent)
            throw new InvalidOperationException("Cannot retry a message that has already been sent.");
        
        RetryCount++;
        if (Status == OutboxStatus.Failed)
            Status = OutboxStatus.Pending;
    }

    public void MarkAsSent()
    {
        if (Status != OutboxStatus.Pending)
            throw new InvalidOperationException("Only pending messages can be marked as sent.");
        
        Status = OutboxStatus.Sent;
        ProcessedAtUtc = DateTimeOffset.UtcNow;
        Error = null;
    }

    public void MarkAsFailed(string error)
    {
        if (Status != OutboxStatus.Pending)
            throw new InvalidOperationException("Only pending messages can be marked as failed.");
        
        Status = OutboxStatus.Failed;
        ProcessedAtUtc = DateTimeOffset.UtcNow;
        Error = error;
    }
}