using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using HerbioMart.Data;
using HerbioMart.Services.Interfaces;
using HerbioMart.ViewModels.Order;

namespace HerbioMart.Services.Implementations
{
    public class CartService : ICartService
    {
        private const string CartSessionKey = "HerbioMart_Cart_Session";
        private const decimal StandardDeliveryFee = 40.00m;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _context;

        public CartService(IHttpContextAccessor httpContextAccessor, AppDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        private ISession Session => _httpContextAccessor.HttpContext!.Session;

        public CartVM GetCart()
        {
            var items = GetRawCartList();
            decimal itemsTotal = items.Sum(i => i.SubTotal);
            decimal deliveryFee = items.Any() ? StandardDeliveryFee : 0.00m;

            return new CartVM
            {
                Items = items,
                ItemsTotal = Math.Round(itemsTotal, 2),
                DeliveryFee = deliveryFee,
                GrandTotal = Math.Round(itemsTotal + deliveryFee, 2)
            };
        }

        public int GetCartItemsCount()
        {
            return GetRawCartList().Count;
        }

        public async Task<bool> AddHerbToCartAsync(int herbId, int grams = 100)
        {
            if (grams <= 0) grams = 100;

            var herb = await _context.Herbs
                .Include(h => h.HerbalistHerbs.Where(hh => hh.IsActive))
                    .ThenInclude(hh => hh.Herbalist)
                        .ThenInclude(h => h.User)
                .FirstOrDefaultAsync(h => h.HerbId == herbId);

            if (herb == null || !herb.HerbalistHerbs.Any())
                return false;

            var vendors = herb.HerbalistHerbs
                .Select(hh => new HerbVendorOptionVM
                {
                    HerbalistId = hh.HerbalistId,
                    HerbalistName = hh.Herbalist?.User?.FullName ?? "Licensed Apothecary",
                    Price = hh.Price
                })
                .OrderBy(v => v.Price)
                .ToList();

            var defaultVendor = vendors.First();
            var cart = GetRawCartList();
            var existing = cart.FirstOrDefault(i => i.ItemType == CartItemType.Herb && i.ItemId == herbId);

            if (existing != null)
            {
                existing.Quantity += grams;
                existing.SubTotal = Math.Round((existing.Quantity * existing.UnitPrice) / 1000m, 2);
                existing.AvailableVendors = vendors;
            }
            else
            {
                cart.Add(new CartItemVM
                {
                    ItemType = CartItemType.Herb,
                    ItemId = herb.HerbId,
                    ItemName = herb.HerbName,
                    HerbalistId = defaultVendor.HerbalistId,
                    HerbalistName = defaultVendor.HerbalistName,
                    UnitPrice = defaultVendor.Price,
                    Quantity = grams,
                    SubTotal = Math.Round((grams * defaultVendor.Price) / 1000m, 2),
                    AvailableVendors = vendors
                });
            }

            SaveRawCartList(cart);
            return true;
        }

        public async Task<bool> AddRecipeToCartAsync(int recipeId, int quantity = 1)
        {
            if (quantity <= 0) quantity = 1;

            var recipe = await _context.Recipes
                .Include(r => r.Herbalist)
                    .ThenInclude(h => h.User)
                .FirstOrDefaultAsync(r => r.RecipeId == recipeId);

            if (recipe == null)
                return false;

            var cart = GetRawCartList();
            var existing = cart.FirstOrDefault(i => i.ItemType == CartItemType.Recipe && i.ItemId == recipeId);

            if (existing != null)
            {
                existing.Quantity += quantity;
                existing.SubTotal = Math.Round(existing.Quantity * recipe.Price, 2);
            }
            else
            {
                cart.Add(new CartItemVM
                {
                    ItemType = CartItemType.Recipe,
                    ItemId = recipe.RecipeId,
                    ItemName = recipe.RecipeName,
                    HerbalistId = recipe.HerbalistId,
                    HerbalistName = recipe.Herbalist?.User?.FullName ?? "Licensed Apothecary",
                    UnitPrice = recipe.Price,
                    Quantity = quantity,
                    SubTotal = Math.Round(quantity * recipe.Price, 2)
                });
            }

            SaveRawCartList(cart);
            return true;
        }

        public bool UpdateQuantityOnly(int index, int newQuantity)
        {
            var cart = GetRawCartList();
            if (index < 0 || index >= cart.Count) return false;

            var item = cart[index];
            if (newQuantity <= 0)
            {
                cart.RemoveAt(index);
            }
            else
            {
                if (item.ItemType == CartItemType.Herb)
                {
                    if (newQuantity < 10) newQuantity = 10;
                    item.Quantity = newQuantity;
                    item.SubTotal = Math.Round((item.Quantity * item.UnitPrice) / 1000m, 2);
                }
                else
                {
                    item.Quantity = newQuantity;
                    item.SubTotal = Math.Round(item.Quantity * item.UnitPrice, 2);
                }
            }

            SaveRawCartList(cart);
            return true;
        }

        public bool ChangeItemVendor(int index, int newHerbalistId)
        {
            var cart = GetRawCartList();
            if (index < 0 || index >= cart.Count) return false;

            var item = cart[index];
            var vendor = item.AvailableVendors.FirstOrDefault(v => v.HerbalistId == newHerbalistId);
            if (vendor == null) return false;

            item.HerbalistId = vendor.HerbalistId;
            item.HerbalistName = vendor.HerbalistName;
            item.UnitPrice = vendor.Price;
            item.SubTotal = Math.Round((item.Quantity * item.UnitPrice) / 1000m, 2);

            SaveRawCartList(cart);
            return true;
        }

        public void RemoveItem(int index)
        {
            var cart = GetRawCartList();
            if (index >= 0 && index < cart.Count)
            {
                cart.RemoveAt(index);
                SaveRawCartList(cart);
            }
        }

        public void ClearCart()
        {
            Session.Remove(CartSessionKey);
        }

        private List<CartItemVM> GetRawCartList()
        {
            var sessionData = Session.GetString(CartSessionKey);
            return string.IsNullOrEmpty(sessionData)
                ? new List<CartItemVM>()
                : JsonSerializer.Deserialize<List<CartItemVM>>(sessionData) ?? new List<CartItemVM>();
        }

        private void SaveRawCartList(List<CartItemVM> list)
        {
            Session.SetString(CartSessionKey, JsonSerializer.Serialize(list));
        }
    }
}