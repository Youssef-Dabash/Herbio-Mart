using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using HerbioMart.Services.Interfaces;
using HerbioMart.ViewModels.PatientProfile;

namespace HerbioMart.Controllers
{
    [Authorize(Roles = "Patient")]
    public class PatientProfileController : Controller
    {
        private readonly IPatientProfileService _profileService;

        public PatientProfileController(IPatientProfileService profileService)
        {
            _profileService = profileService;
        }

        private int GetCurrentPatientId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var id) ? id : 0;
        }

        // GET: PatientProfile/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int patientId = GetCurrentPatientId();
            var profile = await _profileService.GetProfileAsync(patientId);

            if (profile == null)
            {
                return NotFound("لم يتم العثور على بيانات البروفايل.");
            }

            return View(profile);
        }

        // GET: PatientProfile/Update
        [HttpGet]
        public async Task<IActionResult> Update()
        {
            int patientId = GetCurrentPatientId();
            var profile = await _profileService.GetProfileAsync(patientId);

            if (profile == null)
            {
                return NotFound();
            }

           
            var editModel = new EditPatientProfileVM
            {
                FullName = profile.FullName,
                Phone = profile.Phone,
                BirthDate = profile.BirthDate,
                Gender = profile.Gender
                
            };

            return View(editModel);
        }

        // POST: PatientProfile/Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(EditPatientProfileVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            int patientId = GetCurrentPatientId();
            var success = await _profileService.UpdateProfileAsync(model, patientId);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, "حدث خطأ أثناء تحديث البيانات.");
                return View(model);
            }

            TempData["SuccessMessage"] = "تم تحديث بيانات البروفايل بنجاح.";
            return RedirectToAction(nameof(Index));
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete()
        {
            int patientId = GetCurrentPatientId();
            var success = await _profileService.DeleteAccountAsync(patientId);

            if (!success)
            {
                TempData["ErrorMessage"] = "تعذر تعطيل/حذف الحساب حالياً.";
                return RedirectToAction(nameof(Index));
            }

   
            return RedirectToAction("Logout", "Account");
        }
    }
}