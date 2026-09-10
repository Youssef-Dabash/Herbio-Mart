using HerbioMart.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HerbioMart.Data.Configurations;

public class HerbalistHerbConfiguration : IEntityTypeConfiguration<HerbalistHerb>
{
    public void Configure(EntityTypeBuilder<HerbalistHerb> builder)
    {
        builder.HasKey(hh => new { hh.HerbalistId, hh.HerbId });

        builder.Property(hh => hh.Price)
               .HasColumnType("decimal(18,2)");
    }
}