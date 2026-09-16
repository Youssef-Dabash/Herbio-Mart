using HerbioMart.ViewModels.Order;

namespace HerbioMart.Services.Interfaces
{
    public interface ICartService
    {
        CartVM GetCart();
        int GetCartItemsCount();
        Task<bool> AddHerbToCartAsync(int herbId, int grams = 100);
        Task<bool> AddRecipeToCartAsync(int recipeId, int quantity = 1);
        bool UpdateQuantityOnly(int index, int newQuantity);
        bool ChangeItemVendor(int index, int newHerbalistId);
        void RemoveItem(int index);
        void ClearCart();
    }
}