using Microsoft.EntityFrameworkCore;
using HerbioMart.Data;
using HerbioMart.Services.Interfaces;
using HerbioMart.ViewModels.Recipes;
using HerbioMart.ViewModels.Shop;

namespace HerbioMart.Services.Implementations;

public class ShopService : IShopService
{
    private readonly AppDbContext _context;

    public ShopService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ShopIndexVM> GetCatalogAsync(string? searchQuery, string? sortBy, string? disease)
    {
        var herbsQuery = _context.Herbs
            .Include(h => h.HerbalistHerbs.Where(hh => hh.IsActive))
                .ThenInclude(hh => hh.Herbalist)
                    .ThenInclude(h => h.User)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            var query = searchQuery.Trim().ToLower();
            herbsQuery = herbsQuery.Where(h =>
                h.HerbName.ToLower().Contains(query) ||
                (h.ScientificName != null && h.ScientificName.ToLower().Contains(query)) ||
                h.Description.ToLower().Contains(query));
        }

        var rawHerbs = await herbsQuery.ToListAsync();

        var herbsVM = rawHerbs.Select(h => new ShopHerbVM
        {
            Id = h.HerbId,
            Name = h.HerbName,
            ScientificName = h.ScientificName,
            Description = h.Description,
            ImageUrl = h.ImageURL,
            AvailableVendors = h.HerbalistHerbs
                .Where(hh => hh.IsActive)
                .Select(hh => new HerbVendorOptionVM
                {
                    HerbalistId = hh.HerbalistId,
                    HerbalistName = hh.Herbalist?.User?.FullName ?? "Unknown Apothecary",
                    Price = hh.Price
                })
                .OrderBy(v => v.Price) 
                .ToList()
        }).ToList();

        if (sortBy == "priceAsc")
        {
            herbsVM = herbsVM.OrderBy(h => h.AvailableVendors.Any() ? h.AvailableVendors.Min(v => v.Price) : decimal.MaxValue).ToList();
        }
        else if (sortBy == "priceDesc")
        {
            herbsVM = herbsVM.OrderByDescending(h => h.AvailableVendors.Any() ? h.AvailableVendors.Max(v => v.Price) : 0).ToList();
        }

        var recipesQuery = _context.Recipes
            .Include(r => r.Herbalist)
                .ThenInclude(h => h.User)
            .Include(r => r.RecipeDiseases)
                .ThenInclude(rd => rd.Disease)
            .Include(r => r.RecipeHerbs)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            var query = searchQuery.Trim().ToLower();
            recipesQuery = recipesQuery.Where(r =>
                r.RecipeName.ToLower().Contains(query) ||
                r.Description.ToLower().Contains(query));
        }

        if (!string.IsNullOrWhiteSpace(disease))
        {
            var diseaseQuery = disease.Trim().ToLower();
            recipesQuery = recipesQuery.Where(r =>
                r.RecipeDiseases.Any(rd => rd.Disease.DiseaseName.ToLower().Contains(diseaseQuery)));
        }

        recipesQuery = sortBy switch
        {
            "priceAsc" => recipesQuery.OrderBy(r => r.Price),
            "priceDesc" => recipesQuery.OrderByDescending(r => r.Price),
            _ => recipesQuery.OrderByDescending(r => r.RecipeId) 
        };

        var rawRecipes = await recipesQuery.ToListAsync();

        var recipesVM = rawRecipes.Select(r => new ShopRecipeVM
        {
            Id = r.RecipeId,
            HerbalistId = r.HerbalistId, 
            Name = r.RecipeName,
            Description = r.Description,
            Price = r.Price,
            HerbalistName = r.Herbalist?.User?.FullName ?? "Master Herbalist",
            Rating = r.AverageRating,
            ReviewCount = r.TotalRatings,
            HerbsCount = r.RecipeHerbs?.Count ?? 0,
            DiseasesTreated = r.RecipeDiseases?
                .Select(rd => rd.Disease.DiseaseName)
                .ToList() ?? new List<string>()
        }).ToList();

        return new ShopIndexVM
        {
            Herbs = herbsVM,
            Recipes = recipesVM
        };
    }

    public async Task<RecipeDetailsVM?> GetRecipeDetailsAsync(int recipeId)
    {
        var recipe = await _context.Recipes
            .Include(r => r.Herbalist)
                .ThenInclude(h => h.User)
            .Include(r => r.RecipeDiseases)
                .ThenInclude(rd => rd.Disease)
            .Include(r => r.RecipeHerbs)
                .ThenInclude(rh => rh.Herb)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.RecipeId == recipeId);

        if (recipe == null) return null;

        return new RecipeDetailsVM
        {
            RecipeId = recipe.RecipeId,
            RecipeName = recipe.RecipeName,
            Description = recipe.Description,
            Instructions = recipe.Instructions ?? string.Empty,
            Price = recipe.Price,
            AverageRating = recipe.AverageRating,
            TotalRatings = recipe.TotalRatings,
            CreatedDate = recipe.CreatedDate,
            HerbalistName = recipe.Herbalist?.User?.FullName ?? "Licensed Apothecary",
            TargetedDiseases = recipe.RecipeDiseases?
                .Select(rd => rd.Disease.DiseaseName)
                .ToList() ?? new List<string>(),
            Ingredients = recipe.RecipeHerbs?
                .Select(rh => new RecipeIngredientDetailVM
                {
                    HerbName = rh.Herb?.HerbName ?? "Medicinal Botanical",
                    AmountInGrams = rh.Quantity
                }).ToList() ?? new List<RecipeIngredientDetailVM>()
        };
    }
}