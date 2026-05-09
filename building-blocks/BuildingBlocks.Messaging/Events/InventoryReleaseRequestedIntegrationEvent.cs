namespace BuildingBlocks.Messaging.Events;

public sealed record InventoryReleaseRequestedIntegrationEvent(
    Guid OrderId,
    string Reason,
    IReadOnlyCollection<InventoryReleaseRequestedItem> Items
) : IIntegrationEvent
{
    public int Version => 1;
}

public sealed record InventoryReleaseRequestedItem(
    Guid ProductId,
    int Quantity);