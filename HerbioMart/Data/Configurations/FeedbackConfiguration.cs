using HerbioMart.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HerbioMart.Data.Configurations;

public class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
{
    public void Configure(EntityTypeBuilder<Feedback> builder)
    {
        builder.HasOne(f => f.Recipe)
               .WithMany(r => r.Feedbacks)
               .HasForeignKey(f => f.RecipeId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}