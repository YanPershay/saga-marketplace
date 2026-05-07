namespace BuildingBlocks.Messaging.Events;

public sealed record PaymentSucceededIntegrationEvent(
    Guid OrderId,
    Guid PaymentId,
    decimal Amount,
    DateTimeOffset PaidAt
) : IIntegrationEvent
{
    public int Version => 1;
}