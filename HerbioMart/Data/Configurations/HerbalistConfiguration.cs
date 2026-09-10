using HerbioMart.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HerbioMart.Data.Configurations;

public class HerbalistConfiguration : IEntityTypeConfiguration<Herbalist>
{
    public void Configure(EntityTypeBuilder<Herbalist> builder)
    {
        builder.HasOne(h => h.User)
               .WithOne(u => u.Herbalist)
               .HasForeignKey<Herbalist>(h => h.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}