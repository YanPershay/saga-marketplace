using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Order.Infrastructure.Persistence.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Domain.Orders.Order>
{
    public void Configure(EntityTypeBuilder<Domain.Orders.Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.CustomerId)
            .IsRequired();

        builder.Property(o => o.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(o => o.TotalPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.OwnsMany(o => o.OrderItems, itemBuilder =>
        {
            itemBuilder.ToTable("OrderItems");

            itemBuilder.WithOwner()
                .HasForeignKey("OrderId");

            itemBuilder.Property<Guid>("Id");

            itemBuilder.HasKey("Id");

            itemBuilder.Property(i => i.ProductId)
                .IsRequired();

            itemBuilder.Property(i => i.Quantity)
                .IsRequired();

            itemBuilder.Property(i => i.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
        });
    }
}