using HerbioMart.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HerbioMart.Data.Configurations;

public class OrderRecipeConfiguration : IEntityTypeConfiguration<OrderRecipe>
{
    public void Configure(EntityTypeBuilder<OrderRecipe> builder)
    {
        builder.Property(or => or.UnitPrice).HasColumnType("decimal(18,2)");
        builder.Property(or => or.SubTotal).HasColumnType("decimal(18,2)");

        builder.HasOne(or => or.Recipe)
               .WithMany(r => r.OrderRecipes)
               .HasForeignKey(or => or.RecipeId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}