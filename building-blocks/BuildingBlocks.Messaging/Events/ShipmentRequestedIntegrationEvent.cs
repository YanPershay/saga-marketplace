namespace BuildingBlocks.Messaging.Events;

public sealed record ShipmentRequestedIntegrationEvent(
    Guid OrderId
) : IIntegrationEvent
{
    public int Version => 1;
}