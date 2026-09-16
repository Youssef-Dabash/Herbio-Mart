using HerbioMart.ViewModels.Order;

namespace HerbioMart.Services.Interfaces
{
    public interface ICheckoutService
    {
        Task<CheckoutVM?> PrepareCheckoutViewModelAsync(int userId);
        Task<int?> ProcessCheckoutAsync(CheckoutVM model, int userId);
    }
}