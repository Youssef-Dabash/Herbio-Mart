using System.Security.Claims;
using HerbioMart.Services.Interfaces;
using HerbioMart.ViewModels.Recipes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HerbioMart.Controllers;

[Authorize]
public class RecipeController : Controller
{
    private readonly IRecipeService _recipeService;

    public RecipeController(IRecipeService recipeService)
    {
        _recipeService = recipeService;
    }
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        ViewBag.ViewTitle = "All Formulated Recipes";
        ViewBag.IsOwnerView = false;
        var recipes = await _recipeService.GetAllRecipesAsync();
        return View("RecipeList", recipes);
    }

    [HttpGet]
    [Authorize(Roles = "Herbalist")]
    public async Task<IActionResult> MyRecipes()
    {
        ViewBag.ViewTitle = "My Formulated Recipes";
        ViewBag.IsOwnerView = true;
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var recipes = await _recipeService.GetRecipesByHerbalistAsync(userId);
        return View("RecipeList", recipes);
    }
    [HttpGet]
    [Authorize(Roles = "Herbalist")]
    public async Task<IActionResult> Create()
    {
        var model = await _recipeService.PrepareCreateRecipeVMAsync();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Herbalist")]
    public async Task<IActionResult> Create(CreateRecipeVM model)
    {
        if (!ModelState.IsValid)
        {
            // Re-populate dropdowns if validation fails
            var emptyModel = await _recipeService.PrepareCreateRecipeVMAsync();
            model.AvailableHerbs = emptyModel.AvailableHerbs;
            model.AvailableDiseases = emptyModel.AvailableDiseases;
            return View(model);
        }

        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out var userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var result = await _recipeService.CreateRecipeAsync(model, userId);

        if (!result.Success)
        {
            ModelState.AddModelError(nameof(model.RecipeName), result.Message);
            var emptyModel = await _recipeService.PrepareCreateRecipeVMAsync();
            model.AvailableHerbs = emptyModel.AvailableHerbs;
            model.AvailableDiseases = emptyModel.AvailableDiseases;
            return View(model);
        }

        TempData["SuccessMessage"] = result.Message;
        return RedirectToAction("Index", "Herbalist");
    }
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out var userId))
        {
            return RedirectToAction("Login", "Account");
        }

        // Check if the current user is the owner herbalist OR has an approved order containing this recipe
        var isAuthorized = await _recipeService.CanUserViewRecipeDetailsAsync(id, userId);
        if (!isAuthorized)
        {
            TempData["ErrorMessage"] = "You must purchase this proprietary formulation to access its instructions and ingredients monograph.";
            return RedirectToAction(nameof(Index));
        }

        var recipe = await _recipeService.GetRecipeDetailsByIdAsync(id);
        if (recipe == null)
        {
            return NotFound();
        }

        return View(recipe);
    }
}