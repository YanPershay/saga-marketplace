namespace Inventory.Infrastructure.Persistence.Outbox;

public sealed class OutboxMessage
{
    public Guid Id { get; private set; }
    public Guid MessageId { get; private set; }
    public Guid CorrelationId { get; private set; }
    public string EventType { get; private set; } = null!;
    public string RoutingKey { get; private set; } = null!;
    public string Payload { get; private set; } = null!;
    public OutboxMessageStatus Status { get; private set; }
    public DateTimeOffset OccurredAt { get; private set; }
    public DateTimeOffset? ProcessedAt { get; private set; }
    public int RetryCount { get; private set; }
    public string? Error { get; private set; }

    private OutboxMessage()
    {
    }

    public static OutboxMessage Create(
        Guid messageId,
        Guid correlationId,
        string eventType,
        string routingKey,
        string payload,
        DateTimeOffset occurredAt)
    {
        if (messageId == Guid.Empty)
            throw new ArgumentException("MessageId cannot be empty.", nameof(messageId));

        if (correlationId == Guid.Empty)
            throw new ArgumentException("CorrelationId cannot be empty.", nameof(correlationId));

        if (string.IsNullOrWhiteSpace(eventType))
            throw new ArgumentException("EventType cannot be empty.", nameof(eventType));

        if (string.IsNullOrWhiteSpace(routingKey))
            throw new ArgumentException("RoutingKey cannot be empty.", nameof(routingKey));

        if (string.IsNullOrWhiteSpace(payload))
            throw new ArgumentException("Payload cannot be empty.", nameof(payload));

        return new OutboxMessage
        {
            Id = Guid.NewGuid(),
            MessageId = messageId,
            CorrelationId = correlationId,
            EventType = eventType,
            RoutingKey = routingKey,
            Payload = payload,
            Status = OutboxMessageStatus.Pending,
            OccurredAt = occurredAt,
            RetryCount = 0
        };
    }

    public void MarkAsSent()
    {
        Status = OutboxMessageStatus.Sent;
        ProcessedAt = DateTimeOffset.UtcNow;
        Error = null;
    }

    public void MarkAsFailed(string error)
    {
        Status = OutboxMessageStatus.Failed;
        ProcessedAt = DateTimeOffset.UtcNow;
        Error = error;
    }

    public void IncreaseRetry(string error)
    {
        RetryCount++;
        Error = error;
    }
}