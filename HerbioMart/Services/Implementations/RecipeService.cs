using HerbioMart.Data;
using HerbioMart.Models.Entities;
using HerbioMart.Models.Enums;
using HerbioMart.Services.Interfaces;
using HerbioMart.ViewModels.Recipes;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HerbioMart.Services.Implementations
{
    public class RecipeService : IRecipeService
    {
        private readonly AppDbContext _context;

        public RecipeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CreateRecipeVM> PrepareCreateRecipeVMAsync()
        {
            var model = new CreateRecipeVM
            {
                AvailableHerbs = await _context.Herbs
                    .AsNoTracking()
                    .OrderBy(h => h.HerbName)
                    .Select(h => new SelectListItem
                    {
                        Value = h.HerbId.ToString(),
                        Text = $"{h.HerbName} ({h.ScientificName})"
                    })
                    .ToListAsync(),

                AvailableDiseases = await _context.Diseases
                    .AsNoTracking()
                    .OrderBy(d => d.DiseaseName)
                    .Select(d => new SelectListItem
                    {
                        Value = d.DiseaseId.ToString(),
                        Text = d.DiseaseName
                    })
                    .ToListAsync()
            };

            model.Herbs.Add(new RecipeHerbInputVM { HerbId = 0, Quantity = 0 });
            model.SelectedDiseaseIds.Add(0);

            return model;
        }

        public async Task<(bool Success, string Message, int? RecipeId)> CreateRecipeAsync(CreateRecipeVM model, int userId)
        {
            var herbalist = await _context.Herbalists
                .FirstOrDefaultAsync(h => h.UserId == userId);

            if (herbalist == null)
            {
                return (false, "Herbalist profile not found or unauthorized.", null);
            }

            var normalizedName = model.RecipeName.Trim().ToLower();
            var exists = await _context.Recipes
                .AnyAsync(r => r.HerbalistId == herbalist.HerbalistId && r.RecipeName.Trim().ToLower() == normalizedName);

            if (exists)
            {
                return (false, "You already have a recipe formulation registered with this name.", null);
            }

            var distinctHerbs = model.Herbs
                .Where(h => h.HerbId > 0 && h.Quantity > 0)
                .GroupBy(h => h.HerbId)
                .Select(g => g.First())
                .ToList();

            if (!distinctHerbs.Any())
            {
                return (false, "Please select at least one valid medicinal herb.", null);
            }

            var recipe = new Recipe
            {
                HerbalistId = herbalist.HerbalistId,
                RecipeName = model.RecipeName.Trim(),
                Description = model.Description.Trim(),
                Instructions = model.Instructions.Trim(),
                Price = model.Price,
                IsActive = true,
                AverageRating = 0.0,
                TotalRatings = 0,
                CreatedDate = DateTime.UtcNow
            };

            foreach (var herbItem in distinctHerbs)
            {
                recipe.RecipeHerbs.Add(new RecipeHerb
                {
                    HerbId = herbItem.HerbId,
                    Quantity = (float)herbItem.Quantity
                });
            }

            var validDiseaseIds = model.SelectedDiseaseIds
                .Where(id => id > 0)
                .Distinct()
                .ToList();

            foreach (var diseaseId in validDiseaseIds)
            {
                recipe.RecipeDiseases.Add(new RecipeDisease
                {
                    DiseaseId = diseaseId
                });
            }

            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();

            return (true, "Recipe formulation created and listed successfully.", recipe.RecipeId);
        }

        public async Task<List<RecipeListItemVM>> GetAllRecipesAsync()
        {
            return await _context.Recipes
                .AsNoTracking()
                .Where(r => r.IsActive)
                .OrderByDescending(r => r.RecipeId)
                .Select(r => new RecipeListItemVM
                {
                    RecipeId = r.RecipeId,
                    RecipeName = r.RecipeName,
                    Price = r.Price,
                    HerbalistName = r.Herbalist.User.FullName,
                    AverageRating = r.AverageRating,
                    TotalRatings = r.TotalRatings,
                    IngredientsCount = r.RecipeHerbs.Count,
                    TargetedDiseases = r.RecipeDiseases.Select(rd => rd.Disease.DiseaseName).ToList()
                })
                .ToListAsync();
        }

        public async Task<List<RecipeListItemVM>> GetRecipesByHerbalistAsync(int userId)
        {
            return await _context.Recipes
                .AsNoTracking()
                .Where(r => r.Herbalist.UserId == userId)
                .OrderByDescending(r => r.RecipeId)
                .Select(r => new RecipeListItemVM
                {
                    RecipeId = r.RecipeId,
                    RecipeName = r.RecipeName,
                    Price = r.Price,
                    HerbalistName = r.Herbalist.User.FullName,
                    AverageRating = r.AverageRating,
                    TotalRatings = r.TotalRatings,
                    IngredientsCount = r.RecipeHerbs.Count,
                    TargetedDiseases = r.RecipeDiseases.Select(rd => rd.Disease.DiseaseName).ToList()
                })
                .ToListAsync();
        }

        public async Task<RecipeDetailsVM?> GetRecipeDetailsByIdAsync(int recipeId)
        {
            return await _context.Recipes
                .AsNoTracking()
                .Where(r => r.RecipeId == recipeId)
                .Select(r => new RecipeDetailsVM
                {
                    RecipeId = r.RecipeId,
                    RecipeName = r.RecipeName,
                    Description = r.Description,
                    Instructions = r.Instructions,
                    Price = r.Price,
                    AverageRating = r.AverageRating,
                    TotalRatings = r.TotalRatings,
                    CreatedDate = r.CreatedDate,
                    HerbalistName = r.Herbalist.User.FullName,
                    TargetedDiseases = r.RecipeDiseases.Select(rd => rd.Disease.DiseaseName).ToList(),
                    Ingredients = r.RecipeHerbs.Select(rh => new RecipeIngredientDetailVM
                    {
                        HerbId = rh.HerbId,
                        HerbName = rh.Herb.HerbName,
                        ScientificName = rh.Herb.ScientificName,
                        ImageURL = rh.Herb.ImageURL
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }
        public async Task<bool> CanUserViewRecipeDetailsAsync(int recipeId, int userId)
        {
            var isHerbalist = await _context.Herbalists
                .AsNoTracking()
                .AnyAsync(h => h.UserId == userId);

            if (isHerbalist)
                return true;

            var hasPurchased = await _context.OrderRecipes
                .AsNoTracking()
                .AnyAsync(or => or.RecipeId == recipeId &&
                                or.SubOrder.Order.Patient.UserId == userId &&
                                or.SubOrder.Status != SubOrderStatus.Cancelled);

            return hasPurchased;
        }
    }
}