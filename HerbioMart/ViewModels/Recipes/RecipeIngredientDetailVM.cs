namespace HerbioMart.ViewModels.Recipes;

public class RecipeIngredientDetailVM
{
    public int HerbId { get; set; }
    public string HerbName { get; set; } = string.Empty;
    public string ScientificName { get; set; } = string.Empty;
    public string ImageURL { get; set; } = string.Empty;
    public double AmountInGrams { get; set; } // أو int حسب نوع الحقل عندك
}