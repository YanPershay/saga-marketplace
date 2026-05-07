namespace BuildingBlocks.Messaging.Events;

public sealed record PaymentFailedIntegrationEvent(
    Guid OrderId,
    Guid PaymentId,
    decimal Amount,
    string Reason,
    DateTimeOffset FailedAt
) : IIntegrationEvent
{
    public int Version => 1;
}