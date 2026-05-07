namespace BuildingBlocks.Messaging.Events;

public sealed record OrderCreatedIntegrationEvent(
    Guid OrderId,
    Guid CustomerId,
    decimal TotalAmount,
    DateTimeOffset CreatedAt,
    IReadOnlyCollection<OrderCreatedIntegrationEventItem> Items) : IIntegrationEvent
{
    public int Version => 2;
}

public sealed record OrderCreatedIntegrationEventItem(
    Guid ProductId,
    int Quantity);