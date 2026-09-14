using HerbioMart.ViewModels.Shop;

namespace HerbioMart.ViewModels.Home;

public class HomeIndexVM
{
    public List<ShopHerbItemVM> FeaturedHerbs { get; set; } = new();
    public List<ShopRecipeItemVM> FeaturedRecipes { get; set; } = new();
}