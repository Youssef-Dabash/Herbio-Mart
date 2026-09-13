using Microsoft.EntityFrameworkCore;
using HerbioMart.Data;
using HerbioMart.Services.Interfaces;
using HerbioMart.ViewModels.Shop;

namespace HerbioMart.Services.Implementations
{
    public class ShopService : IShopService
    {
        private readonly AppDbContext _context;

        public ShopService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ShopIndexVM> GetCatalogAsync(string? searchQuery = null, string? disease = null, string? sortBy = null)
        {
            var herbsQuery = _context.Herbs.AsNoTracking().AsQueryable();
            var recipesQuery = _context.Recipes.Where(r => r.IsActive).AsNoTracking().AsQueryable();

            // فلترة البحث
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var query = searchQuery.Trim().ToLower();
                herbsQuery = herbsQuery.Where(h => h.HerbName.ToLower().Contains(query) ||
                                                   (h.Description != null && h.Description.ToLower().Contains(query)));

                recipesQuery = recipesQuery.Where(r => r.RecipeName.ToLower().Contains(query) ||
                                                       (r.Description != null && r.Description.ToLower().Contains(query)));
            }

            // الترتيب
            recipesQuery = sortBy switch
            {
                "priceAsc" => recipesQuery.OrderBy(r => r.Price),
                "priceDesc" => recipesQuery.OrderByDescending(r => r.Price),
                _ => recipesQuery.OrderByDescending(r => r.RecipeId)
            };

            herbsQuery = sortBy switch
            {
                "priceAsc" => herbsQuery.OrderBy(h => h.HerbalistHerbs.Select(hh => hh.Price).FirstOrDefault()),
                "priceDesc" => herbsQuery.OrderByDescending(h => h.HerbalistHerbs.Select(hh => hh.Price).FirstOrDefault()),
                _ => herbsQuery.OrderByDescending(h => h.HerbId)
            };

            // Mapping الأعشاب
            var herbsList = await herbsQuery.Select(h => new ShopHerbItemVM
            {
                Id = h.HerbId,
                Name = h.HerbName,
                ScientificName = h.ScientificName,
                Price = h.HerbalistHerbs.Select(hh => hh.Price).FirstOrDefault(),
                ImageUrl = !string.IsNullOrEmpty(h.ImageURL) ? h.ImageURL : "/img/fruite-item-1.jpg",
                Indication = !string.IsNullOrEmpty(h.Benefits) ? h.Benefits : h.Description,
                Description = h.Description
            }).ToListAsync();

            // Mapping الوصفات
            var recipesList = await recipesQuery.Select(r => new ShopRecipeItemVM
            {
                Id = r.RecipeId,
                Name = r.RecipeName,
                HerbalistName = r.Herbalist.User != null ? r.Herbalist.User.FullName : "Youssef Mohammed",
                DiseasesTreated = r.RecipeDiseases.Select(rd => rd.Disease.DiseaseName).ToList(),
                HerbsCount = r.RecipeHerbs.Count,
                Price = r.Price,
                Rating = r.AverageRating,
                ReviewCount = r.TotalRatings,
                Description = r.Description
            }).ToListAsync();

            return new ShopIndexVM
            {
                Herbs = herbsList,
                Recipes = recipesList
            };
        }
    }
}