using HerbioMart.ViewModels.MedicalHistory;

namespace HerbioMart.Services.Interfaces;

public interface IMedicalHistoryService
{
    Task<MedicalHistoryVM?> GetByUserIdAsync(int userId);
    Task<bool> UpsertAsync(MedicalHistoryVM model, int userId);
}