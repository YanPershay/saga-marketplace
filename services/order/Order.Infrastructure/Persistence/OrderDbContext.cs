using Microsoft.EntityFrameworkCore;
using Order.Domain.Orders;

namespace Order.Infrastructure.Persistence;

public sealed class OrderDbContext : DbContext
{
    public DbSet<Domain.Orders.Order> Orders => Set<Domain.Orders.Order>();

    public OrderDbContext(DbContextOptions<OrderDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderDbContext).Assembly);
    }
}