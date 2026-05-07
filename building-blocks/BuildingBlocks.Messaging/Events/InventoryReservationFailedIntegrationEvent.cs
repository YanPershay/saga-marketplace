namespace BuildingBlocks.Messaging.Events;

public sealed record InventoryReservationFailedIntegrationEvent(
    Guid OrderId,
    string Reason,
    IReadOnlyCollection<InventoryReservationFailedItem> Items
) : IIntegrationEvent
{
    public int Version => 1;
}

public sealed record InventoryReservationFailedItem(
    Guid ProductId,
    int RequestedQuantity,
    int AvailableQuantity);