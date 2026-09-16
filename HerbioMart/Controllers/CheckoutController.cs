using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HerbioMart.Services.Interfaces;
using HerbioMart.ViewModels.Order;

namespace HerbioMart.Controllers
{
    [Authorize(Roles = "Patient")]
    public class CheckoutController : Controller
    {
        private readonly ICheckoutService _checkoutService;
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;

        public CheckoutController(
            ICheckoutService checkoutService,
            IOrderService orderService,
            ICartService cartService)
        {
            _checkoutService = checkoutService;
            _orderService = orderService;
            _cartService = cartService;
        }

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await _checkoutService.PrepareCheckoutViewModelAsync(CurrentUserId);
            if (model == null)
            {
                TempData["ErrorMessage"] = "Your prescription cart is empty.";
                return RedirectToAction("Index", "Shop");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(CheckoutVM model)
        {
            model.Cart = _cartService.GetCart();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var orderId = await _checkoutService.ProcessCheckoutAsync(model, CurrentUserId);
            if (orderId == null)
            {
                ModelState.AddModelError(string.Empty, "Unable to complete order. Please verify your profile.");
                return View(model);
            }

            return RedirectToAction(nameof(Confirmation), new { id = orderId.Value });
        }

        [HttpGet]
        public async Task<IActionResult> Confirmation(int id)
        {
            var order = await _orderService.GetOrderDetailsAsync(id, CurrentUserId);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
    }
}