namespace Order.Application.Abstractions;

public interface IOrderRepository
{
    Task AddAsync(Domain.Orders.Order order, CancellationToken cancellationToken = default);
    Task<Domain.Orders.Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}