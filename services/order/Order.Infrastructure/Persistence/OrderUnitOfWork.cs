using Order.Application.Abstractions;
using Order.Infrastructure.Persistence.Outbox;

namespace Order.Infrastructure.Persistence;

public sealed class OrderUnitOfWork : IOrderUnitOfWork
{
    private readonly OrderDbContext _dbContext;

    public OrderUnitOfWork(OrderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddOrderWithOutboxMessageAsync(
        Domain.Orders.Order order,
        string eventType,
        string routingKey,
        string payload,
        Guid messageId,
        Guid correlationId,
        DateTimeOffset occurredAt,
        CancellationToken cancellationToken = default)
    {
        var outboxMessage = OutboxMessage.Create(
            messageId,
            correlationId, eventType, routingKey, payload, occurredAt);
        
        await _dbContext.Orders.AddAsync(order, cancellationToken);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}