using HerbioMart.ViewModels.PatientProfile;
using System.Threading.Tasks;

namespace HerbioMart.Services.Interfaces
{
    public interface IPatientProfileService
    {
        Task<PatientProfileVM?> GetProfileAsync(int patientId);
        Task<bool> UpdateProfileAsync(EditPatientProfileVM model, int patientId);
        Task<bool> DeleteAccountAsync(int patientId); 
    }
}