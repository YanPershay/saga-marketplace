namespace BuildingBlocks.Messaging.Events;

public sealed record OrderCreatedIntegrationEvent(
    Guid OrderId,
    Guid CustomerId,
    decimal TotalAmount,
    DateTimeOffset CreatedAt
) : IIntegrationEvent
{
    public int Version => 1;
}