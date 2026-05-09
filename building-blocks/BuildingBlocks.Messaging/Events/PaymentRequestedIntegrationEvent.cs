namespace BuildingBlocks.Messaging.Events;

public sealed record PaymentRequestedIntegrationEvent(
    Guid OrderId,
    decimal Amount
) : IIntegrationEvent
{
    public int Version => 1;
}