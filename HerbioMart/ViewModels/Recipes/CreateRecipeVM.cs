using Microsoft.AspNetCore.Mvc.Rendering;

namespace HerbioMart.ViewModels.Recipes;

public class CreateRecipeVM
{
    public string RecipeName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public decimal Price { get; set; }

    // Targeted Health Conditions (Multi-select)
    public List<int> SelectedDiseaseIds { get; set; } = new();

    // Compounded Herb Ingredients
    public List<RecipeHerbInputVM> Herbs { get; set; } = new();

    // Dropdown Source Lists
    public List<SelectListItem> AvailableHerbs { get; set; } = new();
    public List<SelectListItem> AvailableDiseases { get; set; } = new();
}