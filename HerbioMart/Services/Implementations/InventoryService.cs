using HerbioMart.Data;
using HerbioMart.Models.Entities;
using HerbioMart.Services.Interfaces;
using HerbioMart.ViewModels.Inventory;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HerbioMart.Services.Implementations;

public class InventoryService : IInventoryService
{
    private readonly AppDbContext _context;

    public InventoryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<InventoryItemVM>> GetHerbalistInventoryAsync(int userId)
    {
        var herbalist = await _context.Herbalists
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.UserId == userId);

        if (herbalist == null) return new List<InventoryItemVM>();

        return await _context.HerbalistHerbs
            .AsNoTracking()
            .Where(hh => hh.HerbalistId == herbalist.HerbalistId)
            .OrderBy(hh => hh.Herb.HerbName)
            .Select(hh => new InventoryItemVM
            {
                HerbId = hh.HerbId,
                HerbName = hh.Herb.HerbName,
                ScientificName = hh.Herb.ScientificName,
                ImageURL = hh.Herb.ImageURL,
                Price = hh.Price,
                IsActive = hh.IsActive
            })
            .ToListAsync();
    }

    public async Task<AddToInventoryVM> PrepareAddToInventoryVMAsync(int userId)
    {
        var herbalist = await _context.Herbalists
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.UserId == userId);

        if (herbalist == null) return new AddToInventoryVM();

        var existingHerbIds = await _context.HerbalistHerbs
            .Where(hh => hh.HerbalistId == herbalist.HerbalistId)
            .Select(hh => hh.HerbId)
            .ToListAsync();

        var availableHerbs = await _context.Herbs
            .AsNoTracking()
            .Where(h => !existingHerbIds.Contains(h.HerbId))
            .OrderBy(h => h.HerbName)
            .Select(h => new SelectListItem
            {
                Value = h.HerbId.ToString(),
                Text = $"{h.HerbName} ({h.ScientificName})"
            })
            .ToListAsync();

        return new AddToInventoryVM
        {
            AvailableCatalogHerbs = availableHerbs
        };
    }

    public async Task<(bool Success, string Message)> AddHerbToInventoryAsync(AddToInventoryVM model, int userId)
    {
        var herbalist = await _context.Herbalists
            .FirstOrDefaultAsync(h => h.UserId == userId);

        if (herbalist == null)
            return (false, "Herbalist account not verified.");

        var alreadyExists = await _context.HerbalistHerbs
            .AnyAsync(hh => hh.HerbalistId == herbalist.HerbalistId && hh.HerbId == model.HerbId);

        if (alreadyExists)
            return (false, "This botanical herb is already listed in your store stock.");

        var inventoryRecord = new HerbalistHerb
        {
            HerbalistId = herbalist.HerbalistId,
            HerbId = model.HerbId,
            Price = model.Price,
            IsActive = model.IsActive
        };

        _context.HerbalistHerbs.Add(inventoryRecord);
        await _context.SaveChangesAsync();

        return (true, "Herb successfully added to your store stock.");
    }

    public async Task<(bool Success, string Message)> UpdateInventoryItemAsync(UpdateInventoryItemVM model, int userId)
    {
        var herbalist = await _context.Herbalists
            .FirstOrDefaultAsync(h => h.UserId == userId);

        if (herbalist == null)
            return (false, "Unauthorized action.");

        var item = await _context.HerbalistHerbs
            .FirstOrDefaultAsync(hh => hh.HerbalistId == herbalist.HerbalistId && hh.HerbId == model.HerbId);

        if (item == null)
            return (false, "Inventory item not found.");

        item.Price = model.Price;
        item.IsActive = model.IsActive;

        await _context.SaveChangesAsync();
        return (true, "Store stock updated successfully.");
    }

    public async Task<(bool Success, string Message)> ToggleStockStatusAsync(int herbId, int userId)
    {
        var herbalist = await _context.Herbalists
            .FirstOrDefaultAsync(h => h.UserId == userId);

        if (herbalist == null)
            return (false, "Unauthorized action.");

        var item = await _context.HerbalistHerbs
            .FirstOrDefaultAsync(hh => hh.HerbalistId == herbalist.HerbalistId && hh.HerbId == herbId);

        if (item == null)
            return (false, "Stock item not found.");

        item.IsActive = !item.IsActive;
        await _context.SaveChangesAsync();

        var status = item.IsActive ? "available for purchase" : "hidden from customers";
        return (true, $"Herb is now {status}.");
    }

    public async Task<(bool Success, string Message)> RemoveFromInventoryAsync(int herbId, int userId)
    {
        var herbalist = await _context.Herbalists
            .FirstOrDefaultAsync(h => h.UserId == userId);

        if (herbalist == null)
            return (false, "Unauthorized action.");

        var item = await _context.HerbalistHerbs
            .FirstOrDefaultAsync(hh => hh.HerbalistId == herbalist.HerbalistId && hh.HerbId == herbId);

        if (item == null)
            return (false, "Stock item not found.");

        _context.HerbalistHerbs.Remove(item);
        await _context.SaveChangesAsync();

        return (true, "Herb removed from your store inventory.");
    }
}