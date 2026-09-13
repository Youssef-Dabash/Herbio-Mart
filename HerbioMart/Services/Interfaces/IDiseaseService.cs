using HerbioMart.ViewModels.Diseases;

namespace HerbioMart.Services.Interfaces;

public interface IDiseaseService
{
    Task<IEnumerable<DiseaseVM>> GetAllDiseasesAsync();
    Task<DiseaseVM?> GetDiseaseByIdAsync(int id);
    Task<(bool Success, string Message, int? DiseaseId)> CreateDiseaseAsync(CreateDiseaseVM model);
}