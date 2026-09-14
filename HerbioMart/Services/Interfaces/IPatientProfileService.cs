using HerbioMart.ViewModels.Patient;
using System.Threading.Tasks;

namespace HerbioMart.Services.Interfaces
{
    public interface IPatientProfileService
    {
        Task<PatientProfileVM?> GetProfileAsync(int userId);
        Task<EditPatientProfileVM?> GetProfileForEditAsync(int userId);
        Task<bool> UpdateProfileAsync(EditPatientProfileVM model, int userId);
        Task<bool> DeleteAccountAsync(int userId);
    }
}