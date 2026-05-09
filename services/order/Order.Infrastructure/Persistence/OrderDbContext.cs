using Microsoft.EntityFrameworkCore;
using Order.Infrastructure.Persistence.Inbox;
using Order.Infrastructure.Persistence.Outbox;

namespace Order.Infrastructure.Persistence;

public sealed class OrderDbContext : DbContext
{
    public DbSet<Domain.Orders.Order> Orders => Set<Domain.Orders.Order>();
    public DbSet<OutboxMessage>  OutboxMessages => Set<OutboxMessage>();
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();

    public OrderDbContext(DbContextOptions<OrderDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderDbContext).Assembly);
    }
}