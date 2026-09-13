using HerbioMart.ViewModels.Shop;

namespace HerbioMart.Services.Interfaces;

public interface IShopService
{
    Task<ShopIndexVM> GetCatalogAsync(string? searchQuery = null, string? disease = null, string? sortBy = null);
}
