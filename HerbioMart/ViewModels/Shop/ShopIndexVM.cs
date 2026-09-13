namespace HerbioMart.ViewModels.Shop;

public class ShopIndexVM
{
    public List<ShopHerbItemVM> Herbs { get; set; } = new();
    public List<ShopRecipeItemVM> Recipes { get; set; } = new();
}