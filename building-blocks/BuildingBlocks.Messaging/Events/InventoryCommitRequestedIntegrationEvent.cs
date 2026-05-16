namespace BuildingBlocks.Messaging.Events;

public sealed record InventoryCommitRequestedIntegrationEvent(
    Guid OrderId,
    string Reason,
    IReadOnlyCollection<InventoryCommitRequestedItem> Items
) : IIntegrationEvent
{
    public int Version => 1;
}

public sealed record InventoryCommitRequestedItem(
    Guid ProductId,
    int Quantity);