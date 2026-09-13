using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using HerbioMart.Services.Interfaces;
using HerbioMart.ViewModels.PatientProfile;

namespace HerbioMart.Controllers
{
    [Authorize(Roles = "Patient")]
    public class PatientController : Controller
    {
        private readonly IPatientProfileService _profileService;

        public PatientController(IPatientProfileService profileService)
        {
            _profileService = profileService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var id) ? id : 0;
        }

        // GET: Patient/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int userId = GetCurrentUserId();
            var profile = await _profileService.GetProfileAsync(userId);

            if (profile == null)
            {
                return NotFound("Patient profile not found.");
            }

            return View(profile);
        }

        // GET: Patient/Update
        [HttpGet]
        public async Task<IActionResult> Update()
        {
            int userId = GetCurrentUserId();
            var editModel = await _profileService.GetProfileForEditAsync(userId);

            if (editModel == null)
            {
                return NotFound();
            }

            return View("Edit", editModel);
        }

        // POST: Patient/Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(EditPatientProfileVM model)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit", model); // Explicitly render Edit.cshtml
            }

            int userId = GetCurrentUserId();
            var success = await _profileService.UpdateProfileAsync(model, userId);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while updating the profile.");
                return View("Edit", model); // Explicitly render Edit.cshtml
            }

            TempData["SuccessMessage"] = "Profile updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Patient/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete()
        {
            int userId = GetCurrentUserId();
            var success = await _profileService.DeleteAccountAsync(userId);

            if (!success)
            {
                TempData["ErrorMessage"] = "Unable to delete account at this moment.";
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction("Logout", "Account");
        }
    }
}