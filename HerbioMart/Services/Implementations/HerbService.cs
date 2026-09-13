using HerbioMart.Data;
using HerbioMart.Models.Entities;
using HerbioMart.Services.Interfaces;
using HerbioMart.ViewModels.Herbalist;
using HerbioMart.ViewModels.Herb;
using Microsoft.EntityFrameworkCore;

namespace HerbioMart.Services.Implementations;

public class HerbService : IHerbService
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public HerbService(AppDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<(bool Success, string Message, int? HerbId)> CreateHerbAsync(CreateHerbVM model, int userId)
    {
        var herbalist = await _context.Herbalists
            .FirstOrDefaultAsync(h => h.UserId == userId);

        if (herbalist == null)
        {
            return (false, "Herbalist profile not found or unauthorized.", null);
        }

        var normalizedScientificName = model.ScientificName.Trim().ToLower();
        var isDuplicate = await _context.Herbs
            .AnyAsync(h => h.ScientificName!.Trim().ToLower() == normalizedScientificName);

        if (isDuplicate)
        {
            return (false, $"A botanical entry with scientific name '{model.ScientificName.Trim()}' already exists.", null);
        }

        string? imagePath = null;
        if (model.ImageFile != null && model.ImageFile.Length > 0)
        {
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "img", "herbs");
            Directory.CreateDirectory(uploadsFolder);

            var fileExtension = Path.GetExtension(model.ImageFile.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var fullFilePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(fullFilePath, FileMode.Create))
            {
                await model.ImageFile.CopyToAsync(stream);
            }

            imagePath = $"/img/herbs/{uniqueFileName}";
        }

        var herb = new Herb
        {
            HerbName = model.HerbName.Trim(),
            ScientificName = model.ScientificName.Trim(),
            Description = model.Description.Trim(),
            Benefits = model.Benefits.Trim(),
            Dosage = model.Dosage.Trim(),
            Warnings = model.Warnings.Trim(),
            ImageURL = imagePath ?? "/img/vegetable-item-6.jpg",
            AddedByHerbalistId = herbalist.HerbalistId
        };

        _context.Herbs.Add(herb);
        await _context.SaveChangesAsync();

        return (true, "Herb registered successfully in the global catalog.", herb.HerbId);
    }

    public async Task<List<HerbListItemVM>> GetAllHerbsAsync()
    {
        return await _context.Herbs
            .AsNoTracking()
            .OrderByDescending(h => h.HerbId)
            .Select(h => new HerbListItemVM
            {
                HerbId = h.HerbId,
                HerbName = h.HerbName,
                ScientificName = h.ScientificName,
                Description = h.Description,
                ImageURL = h.ImageURL,
                AddedByHerbalistName = h.AddedByHerbalist.User.FullName
            })
            .ToListAsync();
    }

    public async Task<HerbDetailsVM?> GetHerbByIdAsync(int herbId)
    {
        return await _context.Herbs
            .AsNoTracking()
            .Where(h => h.HerbId == herbId)
            .Select(h => new HerbDetailsVM
            {
                HerbId = h.HerbId,
                HerbName = h.HerbName,
                ScientificName = h.ScientificName,
                Description = h.Description,
                Benefits = h.Benefits,
                Dosage = h.Dosage,
                Warnings = h.Warnings,
                ImageURL = h.ImageURL,
                AddedByHerbalistName = h.AddedByHerbalist.User.FullName
            })
            .FirstOrDefaultAsync();
    }
}