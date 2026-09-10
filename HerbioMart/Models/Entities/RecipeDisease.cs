namespace HerbioMart.Models.Entities;

public class RecipeDisease
{
    public int RecipeDiseaseId { get; set; }
    public int RecipeId { get; set; }
    public int DiseaseId { get; set; }

    // Navigation Properties
    public virtual Recipe Recipe { get; set; } = null!;
    public virtual Disease Disease { get; set; } = null!;
}