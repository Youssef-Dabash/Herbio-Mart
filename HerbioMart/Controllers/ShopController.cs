namespace HerbioMart.Controllers;
using HerbioMart.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

public class ShopController : Controller
{
    private readonly IShopService _shopService;

    public ShopController(IShopService shopService)
    {
        _shopService = shopService;
    }

    public async Task<IActionResult> Index(string? searchQuery, string? disease, string? sortBy)
    {
        var viewModel = await _shopService.GetCatalogAsync(searchQuery, disease, sortBy);
        return View(viewModel);
    }

    public IActionResult Details(int? id, string type = "Herb")
    {
        if (id == null) return NotFound();
        return View();
    }
}
