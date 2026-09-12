using System.Security.Claims;
using HerbioMart.Services.Interfaces;
using HerbioMart.ViewModels.Herbalist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HerbioMart.Controllers;

public class HerbController : Controller
{
    private readonly IHerbService _herbService;

    public HerbController(IHerbService herbService)
    {
        _herbService = herbService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var herbs = await _herbService.GetAllHerbsAsync();
        return View(herbs);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var herb = await _herbService.GetHerbByIdAsync(id);
        if (herb == null)
        {
            return NotFound();
        }

        return View(herb);
    }

    [HttpGet]
    [Authorize(Roles = "Herbalist")]
    public IActionResult Create()
    {
        return View(new CreateHerbVM());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Herbalist")]
    public async Task<IActionResult> Create(CreateHerbVM model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out var userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var result = await _herbService.CreateHerbAsync(model, userId);

        if (!result.Success)
        {
            ModelState.AddModelError(nameof(model.ScientificName), result.Message);
            return View(model);
        }

        TempData["SuccessMessage"] = result.Message;
        return RedirectToAction(nameof(Index));
    }
}