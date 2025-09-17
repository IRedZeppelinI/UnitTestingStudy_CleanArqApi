using ContadorTabaco.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContadorTabaco.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Quantity)
            .IsRequired();
        
        builder.Property(o => o.TotalCost)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(o => o.OrderDate)
            .IsRequired();

        builder.HasOne(o => o.Product)
            .WithMany()
            .HasForeignKey(o => o.Product.Id)
            .IsRequired();           
        
    }
}

