using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Persistence.Inbox;

public sealed class InboxMessageConfiguration : IEntityTypeConfiguration<InboxMessage>
{
    public void Configure(EntityTypeBuilder<InboxMessage> builder)
    {
        builder.ToTable("InboxMessages");

        builder.HasKey(x => new { x.MessageId, x.ConsumerName });

        builder.Property(x => x.EventType)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(x => x.ConsumerName)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(x => x.ProcessedAt)
            .IsRequired();
    }
}