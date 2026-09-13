using HerbioMart.ViewModels.Herb;
using HerbioMart.ViewModels.Herbalist;

namespace HerbioMart.Services.Interfaces;

public interface IHerbService
{
    Task<(bool Success, string Message, int? HerbId)> CreateHerbAsync(CreateHerbVM model, int userId);
    Task<List<HerbListItemVM>> GetAllHerbsAsync();
    Task<HerbDetailsVM?> GetHerbByIdAsync(int herbId);
}