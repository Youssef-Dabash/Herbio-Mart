namespace HerbioMart.ViewModels.Recipes;

public class RecipeListItemVM
{
    public int RecipeId { get; set; }
    public string RecipeName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string HerbalistName { get; set; } = string.Empty;
    public double AverageRating { get; set; }
    public int TotalRatings { get; set; }
    public List<string> TargetedDiseases { get; set; } = new();
    public int IngredientsCount { get; set; }
}