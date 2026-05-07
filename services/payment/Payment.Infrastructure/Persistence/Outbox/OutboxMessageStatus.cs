namespace Payment.Infrastructure.Persistence.Outbox;

public enum OutboxMessageStatus
{
    Pending = 0,
    Sent = 1,
    Failed = 2
}