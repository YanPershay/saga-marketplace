namespace BuildingBlocks.Messaging.Events;

public sealed record ShipmentFailedIntegrationEvent(
    Guid OrderId,
    string Reason,
    DateTimeOffset FailedAt
) : IIntegrationEvent
{
    public int Version => 1;
}