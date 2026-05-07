namespace BuildingBlocks.Messaging.Events;

public sealed record InventoryReservedIntegrationEvent(
    Guid OrderId,
    IReadOnlyCollection<InventoryReservedItem> Items
) : IIntegrationEvent
{
    public int Version => 1;
}

public sealed record InventoryReservedItem(
    Guid ProductId,
    int Quantity);