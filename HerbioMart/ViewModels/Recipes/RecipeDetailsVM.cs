namespace HerbioMart.ViewModels.Recipes;

public class RecipeDetailsVM
{
    public int RecipeId { get; set; }
    public string RecipeName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public double AverageRating { get; set; }
    public int TotalRatings { get; set; }
    public DateTime CreatedDate { get; set; }
    public string HerbalistName { get; set; } = string.Empty;

    public List<RecipeIngredientDetailVM> Ingredients { get; set; } = new();
    public List<string> TargetedDiseases { get; set; } = new();
}