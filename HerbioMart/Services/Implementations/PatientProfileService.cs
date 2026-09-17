using HerbioMart.Data;
using HerbioMart.Models.Entities;
using HerbioMart.Services.Interfaces;
using HerbioMart.ViewModels.MedicalHistory;
using HerbioMart.ViewModels.Patient;
using HerbioMart.ViewModels.PatientProfile;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;

namespace HerbioMart.Services.Implementations;

public class PatientProfileService : IPatientProfileService
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public PatientProfileService(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async Task<PatientProfileVM?> GetProfileAsync(int userId)
    {
        var patient = await _context.Patients
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (patient == null || patient.User == null)
            return null;

        var medicalRecord = await _context.MedicalHistories
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.PatientId == patient.PatientId);

        return new PatientProfileVM
        {
            FullName = patient.User.FullName,
            UserName = patient.User.UserName,
            Email = patient.User.Email,
            Phone = patient.User.Phone,
            Address = $"{patient.User.Street}, {patient.User.City}, {patient.User.Governorate}".Trim(',', ' '),
            BirthDate = patient.BirthDate,
            Gender = patient.Gender.ToString(),
            ImageUrl = patient.User.ImageUrl,
            MedicalHistory = medicalRecord == null ? null : new MedicalHistoryVM
            {
                MedicalHistoryId = medicalRecord.MedicalHistoryId,
                Diabetes = medicalRecord.Diabetes,
                Hypertension = medicalRecord.Hypertension,
                Asthma = medicalRecord.Asthma,
                Smoker = medicalRecord.Smoker,
                HeartDisease = medicalRecord.HeartDisease,
                KidneyDisease = medicalRecord.KidneyDisease,
                LiverDisease = medicalRecord.LiverDisease,
                OtherNotes = medicalRecord.OtherNotes
            }
        };
    }

    public async Task<EditPatientProfileVM?> GetProfileForEditAsync(int userId)
    {
        var patient = await _context.Patients
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (patient == null || patient.User == null)
            return null;

        return new EditPatientProfileVM
        {
            FullName = patient.User.FullName,
            Phone = patient.User.Phone,
            Street = patient.User.Street,
            City = patient.User.City,
            Governorate = patient.User.Governorate,
            BirthDate = patient.BirthDate,
            Gender = patient.Gender.ToString(),
            ExistingImageUrl = patient.User.ImageUrl
        };
    }

    public async Task<bool> UpdateProfileAsync(EditPatientProfileVM model, int userId)
    {
        var patient = await _context.Patients
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (patient == null || patient.User == null)
            return false;

        if (model.ProfileImage != null && model.ProfileImage.Length > 0)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "users");
            Directory.CreateDirectory(uploadsFolder);

            if (!string.IsNullOrEmpty(patient.User.ImageUrl))
            {
                var oldFilePath = Path.Combine(_env.WebRootPath, patient.User.ImageUrl.TrimStart('/'));
                if (File.Exists(oldFilePath))
                {
                    try { File.Delete(oldFilePath); } catch { /* Ignore */ }
                }
            }

            var uniqueFileName = $"{System.Guid.NewGuid()}_{Path.GetFileName(model.ProfileImage.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await model.ProfileImage.CopyToAsync(fileStream);
            }

            patient.User.ImageUrl = $"/uploads/users/{uniqueFileName}";
        }
        model.ExistingImageUrl = patient.User.ImageUrl;

        patient.User.FullName = model.FullName;
        patient.User.Phone = model.Phone;
        patient.User.Street = model.Street ?? string.Empty;
        patient.User.City = model.City ?? string.Empty;
        patient.User.Governorate = model.Governorate ?? string.Empty;

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
    public async Task<bool> DeleteAccountAsync(int userId)
    {
        var patient = await _context.Patients
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.UserId == userId);

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