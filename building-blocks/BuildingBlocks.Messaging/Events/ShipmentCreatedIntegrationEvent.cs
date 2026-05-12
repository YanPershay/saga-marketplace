namespace BuildingBlocks.Messaging.Events;

public sealed record ShipmentCreatedIntegrationEvent(
    Guid OrderId,
    Guid ShipmentId,
    DateTimeOffset CreatedAt
) : IIntegrationEvent
{
    public int Version => 1;
}