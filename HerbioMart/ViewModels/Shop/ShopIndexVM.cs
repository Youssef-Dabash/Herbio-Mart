namespace HerbioMart.ViewModels.Shop;

public class ShopIndexVM
{
    public List<ShopHerbVM> Herbs { get; set; } = new();
    public List<ShopRecipeVM> Recipes { get; set; } = new();
}
