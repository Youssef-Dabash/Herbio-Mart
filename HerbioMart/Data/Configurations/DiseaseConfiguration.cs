using HerbioMart.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HerbioMart.Data.Configurations;

public class DiseaseConfiguration : IEntityTypeConfiguration<Disease>
{
    public void Configure(EntityTypeBuilder<Disease> builder)
    {
        builder.HasData(
            new Disease
            {
                DiseaseId = 1,
                DiseaseName = "Hypertension",
                Description = "Chronic high blood pressure condition requiring vascular relaxation.",
                Symptoms = "Headache, Dizziness, Shortness of breath"
            },
            new Disease
            {
                DiseaseId = 2,
                DiseaseName = "Type 2 Diabetes",
                Description = "Metabolic condition characterized by elevated blood glucose levels.",
                Symptoms = "Excessive thirst, Frequent urination, Fatigue"
            },
            new Disease
            {
                DiseaseId = 3,
                DiseaseName = "Asthma & Bronchitis",
                Description = "Chronic inflammation of the airways causing breathing difficulty.",
                Symptoms = "Wheezing, Coughing, Chest tightness"
            },
            new Disease
            {
                DiseaseId = 4,
                DiseaseName = "Digestive Disorders (IBS)",
                Description = "Functional gastrointestinal conditions affecting digestion and colon comfort.",
                Symptoms = "Bloating, Abdominal cramps, Indigestion"
            },
            new Disease
            {
                DiseaseId = 5,
                DiseaseName = "Insomnia & Anxiety",
                Description = "Sleep disruption and elevated psychological stress levels.",
                Symptoms = "Difficulty falling asleep, Restlessness, Chronic fatigue"
            }
        );
    }
}