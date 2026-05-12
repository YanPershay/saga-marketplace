namespace Shipping.Infrastructure.Persistence.Inbox;

public sealed class InboxMessage
{
    public Guid MessageId { get; private set; }
    public string EventType { get; private set; } = null!;
    public string ConsumerName { get; private set; } = null!;
    public DateTimeOffset ProcessedAt { get; private set; }

    private InboxMessage()
    {
    }

    public static InboxMessage Create(
        Guid messageId,
        string eventType,
        string consumerName)
    {
        if (messageId == Guid.Empty)
            throw new ArgumentException("MessageId cannot be empty.", nameof(messageId));

        if (string.IsNullOrWhiteSpace(eventType))
            throw new ArgumentException("EventType cannot be empty.", nameof(eventType));

        if (string.IsNullOrWhiteSpace(consumerName))
            throw new ArgumentException("ConsumerName cannot be empty.", nameof(consumerName));

        return new InboxMessage
        {
            MessageId = messageId,
            EventType = eventType,
            ConsumerName = consumerName,
            ProcessedAt = DateTimeOffset.UtcNow
        };
    }
}