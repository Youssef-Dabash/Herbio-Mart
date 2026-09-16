using Microsoft.AspNetCore.Mvc;
using HerbioMart.Services.Interfaces;

namespace HerbioMart.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var cart = _cartService.GetCart();
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddHerbAjax(int herbId, int grams = 100)
        {
            var success = await _cartService.AddHerbToCartAsync(herbId, grams);
            if (!success)
                return Json(new { success = false, message = "This herb is currently out of stock." });

            int count = _cartService.GetCartItemsCount();
            return Json(new { success = true, cartCount = count, message = "Herb added to your cart!" });
        }

        [HttpPost]
        public async Task<IActionResult> AddRecipeAjax(int recipeId, int quantity = 1)
        {
            var success = await _cartService.AddRecipeToCartAsync(recipeId, quantity);
            if (!success)
                return Json(new { success = false, message = "This recipe is currently unavailable." });

            int count = _cartService.GetCartItemsCount();
            return Json(new { success = true, cartCount = count, message = "Recipe added to your cart!" });
        }

        [HttpPost]
        public IActionResult UpdateQuantityAjax(int index, int quantity)
        {
            _cartService.UpdateQuantityOnly(index, quantity);
            var cart = _cartService.GetCart();

            return Json(new
            {
                success = true,
                itemsTotal = cart.ItemsTotal,
                grandTotal = cart.GrandTotal
            });
        }

        [HttpPost]
        public IActionResult ChangeVendorAjax(int index, int herbalistId)
        {
            _cartService.ChangeItemVendor(index, herbalistId);
            var cart = _cartService.GetCart();
            var item = index < cart.Items.Count ? cart.Items[index] : null;

            return Json(new
            {
                success = true,
                unitPrice = item?.UnitPrice ?? 0,
                subTotal = item?.SubTotal ?? 0,
                itemsTotal = cart.ItemsTotal,
                grandTotal = cart.GrandTotal
            });
        }

        [HttpPost]
        public IActionResult Remove(int index)
        {
            _cartService.RemoveItem(index);
            return RedirectToAction(nameof(Index));
        }
    }
}