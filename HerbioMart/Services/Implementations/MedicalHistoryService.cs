using HerbioMart.Data;
using HerbioMart.Models.Entities;
using HerbioMart.Services.Interfaces;
using HerbioMart.ViewModels.MedicalHistory;
using Microsoft.EntityFrameworkCore;

namespace HerbioMart.Services.Implementations;

public class MedicalHistoryService : IMedicalHistoryService
{
    private readonly AppDbContext _context;

    public MedicalHistoryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<MedicalHistoryVM?> GetByUserIdAsync(int userId)
    {
        var patient = await _context.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (patient == null)
            return null;

        var history = await _context.MedicalHistories
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.PatientId == patient.PatientId);

        if (history == null)
        {
            return new MedicalHistoryVM();
        }

        return new MedicalHistoryVM
        {
            MedicalHistoryId = history.MedicalHistoryId,
            Diabetes = history.Diabetes,
            Hypertension = history.Hypertension,
            Asthma = history.Asthma,
            Smoker = history.Smoker,
            HeartDisease = history.HeartDisease,
            KidneyDisease = history.KidneyDisease,
            LiverDisease = history.LiverDisease,
            OtherNotes = history.OtherNotes
        };
    }

    public async Task<bool> UpsertAsync(MedicalHistoryVM model, int userId)
    {
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (patient == null)
            return false;

        var existingHistory = await _context.MedicalHistories
            .FirstOrDefaultAsync(m => m.PatientId == patient.PatientId);

        if (existingHistory == null)
        {
            var newHistory = new MedicalHistory
            {
                PatientId = patient.PatientId,
                Diabetes = model.Diabetes,
                Hypertension = model.Hypertension,
                Asthma = model.Asthma,
                Smoker = model.Smoker,
                HeartDisease = model.HeartDisease,
                KidneyDisease = model.KidneyDisease,
                LiverDisease = model.LiverDisease,
                OtherNotes = model.OtherNotes
            };

            await _context.MedicalHistories.AddAsync(newHistory);
        }
        else
        {
            existingHistory.Diabetes = model.Diabetes;
            existingHistory.Hypertension = model.Hypertension;
            existingHistory.Asthma = model.Asthma;
            existingHistory.Smoker = model.Smoker;
            existingHistory.HeartDisease = model.HeartDisease;
            existingHistory.KidneyDisease = model.KidneyDisease;
            existingHistory.LiverDisease = model.LiverDisease;
            existingHistory.OtherNotes = model.OtherNotes;

            _context.MedicalHistories.Update(existingHistory);
        }

        await _context.SaveChangesAsync();
        return true;
    }
}