namespace Order.Application.Abstractions;

public interface IOrderUnitOfWork
{
    Task AddOrderWithOutboxMessageAsync(
        Order.Domain.Orders.Order order,
        string eventType,
        string routingKey,
        string payload,
        Guid messageId,
        Guid correlationId,
        DateTimeOffset occurredAt,
        CancellationToken cancellationToken = default);
}