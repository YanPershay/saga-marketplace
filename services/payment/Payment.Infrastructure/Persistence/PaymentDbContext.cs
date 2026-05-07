using Microsoft.EntityFrameworkCore;
using Payment.Infrastructure.Persistence.Inbox;
using Payment.Infrastructure.Persistence.Outbox;

namespace Payment.Infrastructure.Persistence;

public sealed class PaymentDbContext : DbContext
{
    public DbSet<Domain.Payments.Payment> Payments => Set<Domain.Payments.Payment>();
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    public PaymentDbContext(DbContextOptions<PaymentDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PaymentDbContext).Assembly);
    }
}