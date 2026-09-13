using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using HerbioMart.Data;
using HerbioMart.Models.Entities;
using HerbioMart.Services.Interfaces;
using HerbioMart.ViewModels.PatientProfile;

namespace HerbioMart.Services.Implementations
{
    public class PatientProfileService : IPatientProfileService
    {
        private readonly AppDbContext _context;

        public PatientProfileService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PatientProfileVM?> GetProfileAsync(int patientId)
        {
            var patient = await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PatientId == patientId);

            if (patient == null || patient.User == null)
                return null;

            return new PatientProfileVM
            {
                FullName = patient.User.FullName,
                UserName = patient.User.UserName,
                Email = patient.User.Email,
                Phone = patient.User.Phone,
                Address = $"{patient.User.Street}, {patient.User.City}, {patient.User.Governorate}",
                BirthDate = patient.BirthDate,
                Gender = patient.Gender.ToString() 
            };
        }

        public async Task<bool> UpdateProfileAsync(EditPatientProfileVM model, int patientId)
        {
            var patient = await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PatientId == patientId);

            if (patient == null || patient.User == null)
                return false;

            patient.User.FullName = model.FullName;
            patient.User.Phone = model.Phone;
            patient.User.Street = model.Street;
            patient.User.City = model.City;
            patient.User.Governorate = model.Governorate;

            
            if (model.BirthDate.HasValue)
            {
                patient.BirthDate = model.BirthDate.Value;
            }

            if (!string.IsNullOrEmpty(model.Gender) && System.Enum.TryParse<HerbioMart.Models.Enums.Gender>(model.Gender, out var parsedGender))
            {
                patient.Gender = parsedGender;
            }

            _context.Patients.Update(patient);
            _context.Users.Update(patient.User);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAccountAsync(int patientId)
        {
            var patient = await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PatientId == patientId);

            if (patient == null)
                return false;

            _context.Patients.Remove(patient);
            if (patient.User != null)
            {
                _context.Users.Remove(patient.User);
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}