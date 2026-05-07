using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Persistence.Outbox;

public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");
        
        builder.HasKey(p => p.Id);

        builder.Property(p => p.MessageId)
            .IsRequired();
        
        builder.Property(p => p.CorrelationId)
            .IsRequired();
        
        builder.Property(p => p.EventType)
            .HasMaxLength(250)
            .IsRequired();
        
        builder.Property(p => p.RoutingKey)
            .HasMaxLength(250)
            .IsRequired();
        
        builder.Property(p => p.Payload)
            .HasColumnType("jsonb")
            .IsRequired();
        
        builder.Property(p => p.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.OccurredAt)
            .IsRequired();

        builder.Property(p => p.ProcessedAt);
        
        builder.Property(p => p.RetryCount)
            .IsRequired();

        builder.Property(p => p.Error)
            .HasMaxLength(4000);

        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.MessageId)
            .IsUnique();
    }
}