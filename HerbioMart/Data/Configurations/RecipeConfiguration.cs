using HerbioMart.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HerbioMart.Data.Configurations;

public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> builder)
    {
        builder.Property(r => r.Price)
               .HasColumnType("decimal(18,2)");

        builder.HasOne(r => r.Herbalist)
               .WithMany(h => h.Recipes)
               .HasForeignKey(r => r.HerbalistId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}