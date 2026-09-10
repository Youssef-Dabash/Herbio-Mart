using HerbioMart.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HerbioMart.Data.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.HasOne(p => p.User)
               .WithOne(u => u.Patient)
               .HasForeignKey<Patient>(p => p.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.MedicalHistory)
               .WithOne(m => m.Patient)
               .HasForeignKey<MedicalHistory>(m => m.PatientId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}