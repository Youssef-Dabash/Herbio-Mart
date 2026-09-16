using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using HerbioMart.Services.Interfaces;
using HerbioMart.ViewModels.Feedback;

namespace HerbioMart.Controllers
{
    [Authorize(Roles = "Patient")]
    public class FeedbackController : Controller
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        private int GetCurrentPatientId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(claim, out int patientId) ? patientId : 0;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task Add(CreateFeedbackVM model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Details", "Recipe", new { id = model.RecipeId });
            }

            int patientId = GetCurrentPatientId();
            await _feedbackService.AddReviewAsync(model, patientId);

            return RedirectToAction("Details", "Recipe", new { id = model.RecipeId });
        }

        [HttpGet]
        public async Task Edit(int id)
        {
            return View("Update", new EditFeedbackVM { FeedbackId = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task Edit(EditFeedbackVM model)
        {
            if (!ModelState.IsValid)
            {
                return View("Update", model);
            }

            int patientId = GetCurrentPatientId();
            bool result = await _feedbackService.UpdateReviewAsync(model, patientId);

            if (!result)
            {
                return Unauthorized();
            }

            return RedirectToAction("Index", "PatientProfile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task Delete(int id)
        {
            int patientId = GetCurrentPatientId();
            bool result = await _feedbackService.DeleteReviewAsync(id, patientId);

            if (!result)
            {
                return Unauthorized();
            }

            return RedirectToAction("Index", "PatientProfile");
        }
    }
}
