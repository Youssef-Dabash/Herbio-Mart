using System.Security.Claims;
using HerbioMart.Services.Interfaces;
using HerbioMart.ViewModels.Recipes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HerbioMart.Controllers
{
    [Authorize]
    public class RecipeController : Controller
    {
        private readonly IRecipeService _recipeService;

        public RecipeController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // GET: /Recipe (Public Catalog)
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            ViewBag.ViewTitle = "All Formulated Recipes";
            ViewBag.ViewSubtitle = "Browse prepared medicinal herbal remedies compounded by certified apothecaries.";
            ViewBag.IsOwnerView = false;

            var recipes = await _recipeService.GetAllRecipesAsync();
            return View("RecipeList", recipes);
        }

        // GET: /Recipe/MyRecipes (Herbalist's own formulas)
        [HttpGet]
        [Authorize(Roles = "Herbalist")]
        public async Task<IActionResult> MyRecipes()
        {
            ViewBag.ViewTitle = "My Formulated Recipes";
            ViewBag.ViewSubtitle = "Manage proprietary therapeutic formulations compounded under your licensed dispensary.";
            ViewBag.IsOwnerView = true;

            var recipes = await _recipeService.GetRecipesByHerbalistAsync(CurrentUserId);
            return View("RecipeList", recipes);
        }

        // GET: /Recipe/Create
        [HttpGet]
        [Authorize(Roles = "Herbalist")]
        public async Task<IActionResult> Create()
        {
            var model = await _recipeService.PrepareCreateRecipeVMAsync();
            return View(model);
        }

        // POST: /Recipe/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Herbalist")]
        public async Task<IActionResult> Create(CreateRecipeVM model)
        {
            if (!ModelState.IsValid)
            {
                var emptyModel = await _recipeService.PrepareCreateRecipeVMAsync();
                model.AvailableHerbs = emptyModel.AvailableHerbs;
                model.AvailableDiseases = emptyModel.AvailableDiseases;
                return View(model);
            }

            var result = await _recipeService.CreateRecipeAsync(model, CurrentUserId);

            if (!result.Success)
            {
                ModelState.AddModelError(nameof(model.RecipeName), result.Message);
                var emptyModel = await _recipeService.PrepareCreateRecipeVMAsync();
                model.AvailableHerbs = emptyModel.AvailableHerbs;
                model.AvailableDiseases = emptyModel.AvailableDiseases;
                return View(model);
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(MyRecipes));
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
                TempData["ProtectedMonographTitle"] = "Proprietary Formulation Monograph Protected";
                TempData["ProtectedMonographMessage"] = "To safeguard the intellectual property of certified apothecaries, full preparation steps, precise ratios, and compounding instructions are accessible exclusively to patients who have purchased this formulation. Add it to your cart to unlock full access upon delivery.";
                return RedirectToAction(nameof(Index));
            }

            var recipe = await _recipeService.GetRecipeDetailsByIdAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }
        // GET: /Recipe/Details/{id}
        //[HttpGet]
        //public async Task<IActionResult> Details(int id)
        //{
        //    var isAuthorized = await _recipeService.CanUserViewRecipeDetailsAsync(id, CurrentUserId);
        //    if (!isAuthorized)
        //    {
        //        TempData["ErrorMessage"] = "You must purchase this proprietary formulation to access its instructions and ingredients monograph.";
        //        return RedirectToAction(nameof(Index));
        //    }

        //    var recipe = await _recipeService.GetRecipeDetailsByIdAsync(id);
        //    if (recipe == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(recipe);
        //}
    }
}