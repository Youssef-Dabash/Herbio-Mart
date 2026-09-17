using AutoMapper;
using HerbioMart.Data;
using HerbioMart.Models.Entities;
using HerbioMart.Services.Interfaces;
using HerbioMart.ViewModels.Diseases;
using Microsoft.EntityFrameworkCore;

namespace HerbioMart.Services.Implementations;

public class DiseaseService : IDiseaseService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public DiseaseService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<DiseaseVM>> GetAllDiseasesAsync()
    {
        var diseases = await _context.Diseases
            .AsNoTracking()
            .ToListAsync();

        return _mapper.Map<IEnumerable<DiseaseVM>>(diseases);
    }

    public async Task<DiseaseVM?> GetDiseaseByIdAsync(int id)
    {
        var disease = await _context.Diseases
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.DiseaseId == id);

        if (disease == null)
        {
            return null;
        }

        return _mapper.Map<DiseaseVM>(disease);
    }

    public async Task<(bool Success, string Message, int? DiseaseId)> CreateDiseaseAsync(CreateDiseaseVM model)
    {
        var normalizedName = model.DiseaseName.Trim().ToLower();
        var exists = await _context.Diseases
            .AnyAsync(d => d.DiseaseName.Trim().ToLower() == normalizedName);

        if (exists)
        {
            return (false, $"A health condition with the name '{model.DiseaseName.Trim()}' already exists in the registry.", null);
        }

        var disease = _mapper.Map<Disease>(model);
        disease.DiseaseName = model.DiseaseName.Trim();
        disease.Description = model.Description.Trim();
        disease.Symptoms = model.Symptoms.Trim();

        _context.Diseases.Add(disease);
        await _context.SaveChangesAsync();

        return (true, "Disease condition registered successfully.", disease.DiseaseId);
    }
}