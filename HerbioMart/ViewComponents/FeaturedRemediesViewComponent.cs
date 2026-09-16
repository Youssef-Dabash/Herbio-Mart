using HerbioMart.Data;
using HerbioMart.ViewModels.Home;
using HerbioMart.ViewModels.Shop;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HerbioMart.ViewComponents;

public class FeaturedRemediesViewComponent : ViewComponent
{
    private readonly AppDbContext _context;

    public FeaturedRemediesViewComponent(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var herbs = await _context.Herbs
            .AsNoTracking()
            .Take(8)
            .Select(h => new ShopHerbVM
            {
                Id = h.HerbId,
                Name = h.HerbName,
                ScientificName = h.ScientificName,
                ImageUrl = !string.IsNullOrEmpty(h.ImageURL) ? h.ImageURL : "/img/fruite-item-1.jpg",
                Description = h.Description
            })
            .ToListAsync();

        var recipes = await _context.Recipes
            .Where(r => r.IsActive)
            .AsNoTracking()
            .Take(6)
            .Select(r => new ShopRecipeVM
            {
                Id = r.RecipeId,
                Name = r.RecipeName,
                HerbalistName = r.Herbalist.User != null ? r.Herbalist.User.FullName : "Certified Herbalist",
                DiseasesTreated = r.RecipeDiseases.Select(rd => rd.Disease.DiseaseName).ToList(),
                HerbsCount = r.RecipeHerbs.Count,
                Price = r.Price,
                Rating = r.AverageRating > 0 ? r.AverageRating : 0,
                ReviewCount = r.TotalRatings,
                Description = r.Description
            })
            .ToListAsync();

        var model = new HomeIndexVM
        {
            FeaturedHerbs = herbs,
            FeaturedRecipes = recipes
        };

        return View("~/Views/Shared/_FeaturedRemedies.cshtml", model);
    }
}