using HerbioMart.Models.Enums;
using HerbioMart.ViewModels.Herbalist;

namespace HerbioMart.Services.Interfaces;

public interface IHerbalistOrderService
{
    Task<List<HerbalistSubOrderListItemVM>> GetHerbalistSubOrdersAsync(int userId, SubOrderStatus? statusFilter = null);
    Task<HerbalistSubOrderDetailsVM?> GetSubOrderDetailsAsync(int subOrderId, int userId);
    Task<bool> UpdateSubOrderStatusAsync(int subOrderId, SubOrderStatus newStatus, int userId);
}