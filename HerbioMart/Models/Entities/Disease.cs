namespace HerbioMart.Models.Entities;

public class Disease
{
    public int DiseaseId { get; set; }
    public string DiseaseName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Symptoms { get; set; } = string.Empty;

    // Navigation Properties
    public virtual ICollection<RecipeDisease> RecipeDiseases { get; set; } = new List<RecipeDisease>();
}