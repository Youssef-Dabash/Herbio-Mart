using HerbioMart.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HerbioMart.Data.Configurations;

public class SubOrderConfiguration : IEntityTypeConfiguration<SubOrder>
{
    public void Configure(EntityTypeBuilder<SubOrder> builder)
    {
        builder.Property(s => s.SubTotal)
               .HasColumnType("decimal(18,2)");

        builder.HasOne(s => s.Herbalist)
               .WithMany(h => h.SubOrders)
               .HasForeignKey(s => s.HerbalistId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}