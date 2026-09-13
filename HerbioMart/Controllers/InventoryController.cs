using System.Security.Claims;
using HerbioMart.Services.Interfaces;
using HerbioMart.ViewModels.Inventory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HerbioMart.Controllers;

[Authorize(Roles = "Herbalist")]
public class InventoryController : Controller
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    private int GetCurrentUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var items = await _inventoryService.GetHerbalistInventoryAsync(GetCurrentUserId());
        return View(items);
    }

    [HttpGet]
    public async Task<IActionResult> Add()
    {
        var model = await _inventoryService.PrepareAddToInventoryVMAsync(GetCurrentUserId());
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(AddToInventoryVM model)
    {
        if (!ModelState.IsValid)
        {
            var prep = await _inventoryService.PrepareAddToInventoryVMAsync(GetCurrentUserId());
            model.AvailableCatalogHerbs = prep.AvailableCatalogHerbs;
            return View(model);
        }

        var result = await _inventoryService.AddHerbToInventoryAsync(model, GetCurrentUserId());
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            var prep = await _inventoryService.PrepareAddToInventoryVMAsync(GetCurrentUserId());
            model.AvailableCatalogHerbs = prep.AvailableCatalogHerbs;
            return View(model);
        }

        TempData["SuccessMessage"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateItem(UpdateInventoryItemVM model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Invalid price or parameters.";
            return RedirectToAction(nameof(Index));
        }

        var result = await _inventoryService.UpdateInventoryItemAsync(model, GetCurrentUserId());
        if (result.Success)
            TempData["SuccessMessage"] = result.Message;
        else
            TempData["ErrorMessage"] = result.Message;

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(int herbId)
    {
        var result = await _inventoryService.ToggleStockStatusAsync(herbId, GetCurrentUserId());
        TempData["SuccessMessage"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int herbId)
    {
        var result = await _inventoryService.RemoveFromInventoryAsync(herbId, GetCurrentUserId());
        TempData["SuccessMessage"] = result.Message;
        return RedirectToAction(nameof(Index));
    }
}