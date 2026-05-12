namespace BuildingBlocks.Messaging.Events;

public sealed record PaymentRefundRequestedIntegrationEvent(
    Guid OrderId,
    string Reason
) : IIntegrationEvent
{
    public int Version => 1;
}