using AutoMapper;
using HerbioMart.Services.Interfaces;
using HerbioMart.ViewModels.Patient;
using HerbioMart.ViewModels.PatientProfile;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HerbioMart.Controllers;

[Authorize(Roles = "Patient")]
public class PatientController : Controller
{
    private readonly IPatientProfileService _patientService;

    public PatientController(IPatientProfileService patientService)
    {
        _patientService = patientService;
    }

    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var profile = await _patientService.GetProfileAsync(CurrentUserId);
        if (profile == null) return NotFound();

        return View(profile);
    }

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var model = await _patientService.GetProfileForEditAsync(CurrentUserId);
        if (model == null) return NotFound();

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditPatientProfileVM model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var isSuccess = await _patientService.UpdateProfileAsync(model, CurrentUserId);
        if (!isSuccess)
        {
            ModelState.AddModelError(string.Empty, "Unable to update profile.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, CurrentUserId.ToString()),
            new Claim(ClaimTypes.Name, User.Identity!.Name!),
            new Claim("FullName", model.FullName),
            new Claim(ClaimTypes.Role, "Patient"),
            new Claim("ProfilePicture", model.ExistingImageUrl ?? "")
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            new AuthenticationProperties { IsPersistent = true });

        TempData["SuccessMessage"] = "Profile updated successfully!";
        return RedirectToAction(nameof(Index));
    }
}