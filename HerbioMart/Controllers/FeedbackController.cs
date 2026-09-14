using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HerbioMart.Services.Interfaces;
using HerbioMart.ViewModels.Feedback;

namespace HerbioMart.Controllers;

public class FeedbackController : Controller
{
    private readonly IFeedbackService _feedbackService;

    public FeedbackController(IFeedbackService feedbackService)
    {
        _feedbackService = feedbackService;
    }

    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var feedbacks = await _feedbackService.GetAllFeedbacksAsync();
        return View(feedbacks);
    }

    [Authorize(Roles = "Patient")]
    [HttpGet]
    public async Task<IActionResult> MyFeedbacks()
    {
        var myFeedbacks = await _feedbackService.GetFeedbacksByPatientUserIdAsync(CurrentUserId);
        return View(myFeedbacks);
    }

    // صفحة Create (تخدم الجديد والتعديل)
    [Authorize(Roles = "Patient")]
    [HttpGet]
    public async Task<IActionResult> Create(int recipeId)
    {
        var model = await _feedbackService.GetFeedbackFormAsync(recipeId, CurrentUserId);
        if (model == null) return NotFound("Recipe not found.");

        return View(model);
    }

    // حفظ الـ Upsert
    [Authorize(Roles = "Patient")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FeedbackFormVM model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var success = await _feedbackService.SaveFeedbackAsync(model, CurrentUserId);
        if (!success)
        {
            ModelState.AddModelError(string.Empty, "An error occurred while saving your feedback.");
            return View(model);
        }

        TempData["SuccessMessage"] = model.FeedbackId > 0
            ? "Your review has been updated successfully!"
            : "Your review has been shared successfully!";

        return RedirectToAction(nameof(MyFeedbacks));
    }

    [Authorize(Roles = "Patient")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _feedbackService.DeleteFeedbackAsync(id, CurrentUserId);
        if (!success) return NotFound();

        TempData["SuccessMessage"] = "Review removed successfully.";
        return RedirectToAction(nameof(MyFeedbacks));
    }
}