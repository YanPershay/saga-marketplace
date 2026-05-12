using Microsoft.EntityFrameworkCore;
using Shipping.Domain.Shipments;
using Shipping.Infrastructure.Persistence.Inbox;
using Shipping.Infrastructure.Persistence.Outbox;

namespace Shipping.Infrastructure.Persistence;

public sealed class ShippingDbContext : DbContext
{
    public DbSet<Shipment> Shipments => Set<Shipment>();
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    
    public ShippingDbContext(DbContextOptions<ShippingDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ShippingDbContext).Assembly);
    }
}
