using Microsoft.EntityFrameworkCore;
using Order.Domain.Orders;
using Order.Infrastructure.Persistence.Outbox;

namespace Order.Infrastructure.Persistence;

public sealed class OrderDbContext : DbContext
{
    public DbSet<Domain.Orders.Order> Orders => Set<Domain.Orders.Order>();
    public DbSet<OutboxMessage>  OutboxMessages => Set<OutboxMessage>();

    public OrderDbContext(DbContextOptions<OrderDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderDbContext).Assembly);
    }
}