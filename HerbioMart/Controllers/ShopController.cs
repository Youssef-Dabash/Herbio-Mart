using Microsoft.AspNetCore.Mvc;
using HerbioMart.Services.Interfaces;

namespace HerbioMart.Controllers;

public class ShopController : Controller
{
    private readonly IShopService _shopService;

    public ShopController(IShopService shopService)
    {
        _shopService = shopService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? searchQuery, string? sortBy, string? disease)
    {
        var catalog = await _shopService.GetCatalogAsync(searchQuery, sortBy, disease);
        return View("Catalog", catalog); 
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var remedy = await _shopService.GetRecipeDetailsAsync(id);
        if (remedy == null)
        {
            return NotFound("Formulated remedy was not found.");
        }

        return View(remedy);
    }
}