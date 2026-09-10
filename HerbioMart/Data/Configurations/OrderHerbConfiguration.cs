using HerbioMart.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HerbioMart.Data.Configurations;

public class OrderHerbConfiguration : IEntityTypeConfiguration<OrderHerb>
{
    public void Configure(EntityTypeBuilder<OrderHerb> builder)
    {
        builder.Property(oh => oh.UnitPrice).HasColumnType("decimal(18,2)");
        builder.Property(oh => oh.SubTotal).HasColumnType("decimal(18,2)");

        builder.HasOne(oh => oh.Herb)
               .WithMany(h => h.OrderHerbs)
               .HasForeignKey(oh => oh.HerbId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}