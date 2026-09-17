using HerbioMart.Services.Interfaces;
using HerbioMart.ViewModels.MedicalHistory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HerbioMart.Controllers;

[Authorize(Roles = "Patient")]
public class MedicalHistoryController : Controller
{
    private readonly IMedicalHistoryService _medicalHistoryService;

    public MedicalHistoryController(IMedicalHistoryService medicalHistoryService)
    {
        _medicalHistoryService = medicalHistoryService;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var id) ? id : 0;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        int userId = GetCurrentUserId();
        var history = await _medicalHistoryService.GetByUserIdAsync(userId);

        if (history == null)
        {
            return NotFound("Patient record was not found.");
        }

        return View(history);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(MedicalHistoryVM model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        int userId = GetCurrentUserId();
        var success = await _medicalHistoryService.UpsertAsync(model, userId);

        if (!success)
        {
            ModelState.AddModelError(string.Empty, "Failed to save medical history.");
            return View(model);
        }

        TempData["SuccessMessage"] = "Medical history successfully updated.";
        return RedirectToAction(nameof(Index), "Patient");
    }
}