using HerbioMart.ViewModels.Inventory;

namespace HerbioMart.Services.Interfaces;

public interface IInventoryService
{
    Task<List<InventoryItemVM>> GetHerbalistInventoryAsync(int userId);
    Task<AddToInventoryVM> PrepareAddToInventoryVMAsync(int userId);
    Task<(bool Success, string Message)> AddHerbToInventoryAsync(AddToInventoryVM model, int userId);
    Task<(bool Success, string Message)> UpdateInventoryItemAsync(UpdateInventoryItemVM model, int userId);
    Task<(bool Success, string Message)> ToggleStockStatusAsync(int herbId, int userId);
    Task<(bool Success, string Message)> RemoveFromInventoryAsync(int herbId, int userId);
}