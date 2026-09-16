using HerbioMart.Models.Entities;
using HerbioMart.ViewModels.Shop;

namespace HerbioMart.Services.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<HerbVendorOptionVM>> GetAvailableVendorsForHerbAsync(int herbId);
        Task<Order?> GetOrderDetailsAsync(int orderId, int userId);
        Task<IEnumerable<Order>> GetPatientOrdersAsync(int userId);
    }
}