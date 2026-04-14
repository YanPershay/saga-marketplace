namespace BuildingBlocks.Inbox;

public sealed class InboxMessage
{
    public Guid Id { get; set; }
    public Guid MessageId { get; }
    public DateTimeOffset ProcessedAtUtc { get; private set; }
    
    public InboxMessage(Guid messageId)
    {
        if (messageId == Guid.Empty)
            throw new ArgumentException("MessageId cannot be empty.", nameof(messageId));
        
        Id = Guid.NewGuid();
        MessageId = messageId;
        ProcessedAtUtc = DateTimeOffset.UtcNow;
    }
}