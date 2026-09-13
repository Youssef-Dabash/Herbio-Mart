using HerbioMart.ViewModels.Recipes;

namespace HerbioMart.Services.Interfaces;

public interface IRecipeService
{
    Task<CreateRecipeVM> PrepareCreateRecipeVMAsync();
    Task<(bool Success, string Message, int? RecipeId)> CreateRecipeAsync(CreateRecipeVM model, int userId);
    Task<List<RecipeListItemVM>> GetAllRecipesAsync();
    Task<List<RecipeListItemVM>> GetRecipesByHerbalistAsync(int userId);
    Task<RecipeDetailsVM?> GetRecipeDetailsByIdAsync(int recipeId);
    Task<bool> CanUserViewRecipeDetailsAsync(int recipeId, int userId);
}