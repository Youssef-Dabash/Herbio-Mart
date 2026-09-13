using HerbioMart.Services.Interfaces;
using HerbioMart.ViewModels.Diseases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HerbioMart.Controllers;

[Authorize(Roles = "Herbalist")]
public class DiseaseController : Controller
{
    private readonly IDiseaseService _diseaseService;

    public DiseaseController(IDiseaseService diseaseService)
    {
        _diseaseService = diseaseService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var diseases = await _diseaseService.GetAllDiseasesAsync();
        return View(diseases);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateDiseaseVM());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateDiseaseVM model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _diseaseService.CreateDiseaseAsync(model);

        if (!result.Success)
        {
            ModelState.AddModelError(nameof(model.DiseaseName), result.Message);
            return View(model);
        }

        TempData["SuccessMessage"] = result.Message;
        return RedirectToAction("Index", "Herbalist");
    }
}