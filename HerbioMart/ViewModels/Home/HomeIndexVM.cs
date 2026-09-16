using HerbioMart.ViewModels.Shop;

namespace HerbioMart.ViewModels.Home;

public class HomeIndexVM
{
    public List<ShopHerbVM> FeaturedHerbs { get; set; } = new();
    public List<ShopRecipeVM> FeaturedRecipes { get; set; } = new();
}