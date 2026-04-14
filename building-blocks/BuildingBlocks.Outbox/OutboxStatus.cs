namespace BuildingBlocks.Outbox;

public enum OutboxStatus
{
    Pending,
    Sent,
    Failed,
}