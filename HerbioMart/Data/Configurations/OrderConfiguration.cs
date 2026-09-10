using HerbioMart.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HerbioMart.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.Property(o => o.ItemsTotal).HasColumnType("decimal(18,2)");
        builder.Property(o => o.DeliveryFee).HasColumnType("decimal(18,2)");
        builder.Property(o => o.TotalPrice).HasColumnType("decimal(18,2)");
    }
}