using Microsoft.EntityFrameworkCore;
using HerbioMart.Data;
using HerbioMart.Models.Entities;
using HerbioMart.Models.Enums;
using HerbioMart.Services.Interfaces;
using HerbioMart.ViewModels.Order;

namespace HerbioMart.Services.Implementations
{
    public class CheckoutService : ICheckoutService
    {
        private readonly AppDbContext _context;
        private readonly ICartService _cartService;

        public CheckoutService(AppDbContext context, ICartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }

        public async Task<CheckoutVM?> PrepareCheckoutViewModelAsync(int userId)
        {
            var cart = _cartService.GetCart();
            if (!cart.Items.Any())
                return null;

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            return new CheckoutVM
            {
                Cart = cart,
                FullName = user?.FullName ?? string.Empty,
                Email = user?.Email ?? string.Empty,
                Phone = user?.Phone ?? string.Empty
            };
        }

        public async Task<int?> ProcessCheckoutAsync(CheckoutVM model, int userId)
        {
            var cart = _cartService.GetCart();
            if (!cart.Items.Any())
                return null;

            var patient = await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient == null)
                return null;

            var recipientName = string.IsNullOrWhiteSpace(model.FullName) ? patient.User.FullName : model.FullName;
            var recipientPhone = string.IsNullOrWhiteSpace(model.Phone) ? patient.User.Phone : model.Phone;
            var notes = string.IsNullOrWhiteSpace(model.Notes) ? "None" : model.Notes.Trim();

            // Store designated order shipping destination
            //var shippingDetails = $"{recipientName} | Phone: {recipientPhone} | Delivery Address: {model.ShippingAddress.Trim()} | Notes: {notes}";
            var cleanAddress = model.ShippingAddress?.Trim() ?? string.Empty;
            // 1. Master Order
            var order = new Order
            {
                PatientId = patient.PatientId,
                OrderDate = DateTime.UtcNow,
                OrderStatus = OrderStatus.Pending,
                ShippingAddress = cleanAddress,
                DeliveryFee = cart.DeliveryFee,
                ItemsTotal = cart.ItemsTotal,
                TotalPrice = cart.GrandTotal
            };

            // 2. SubOrders grouped by Herbalist
            var vendorGroups = cart.Items.GroupBy(item => item.HerbalistId);

            foreach (var group in vendorGroups)
            {
                var subOrder = new SubOrder
                {
                    HerbalistId = group.Key,
                    Status = SubOrderStatus.Pending,
                    SubTotal = group.Sum(i => i.SubTotal),
                    Order = order
                };

                foreach (var item in group)
                {
                    if (item.ItemType == CartItemType.Herb)
                    {
                        subOrder.OrderHerbs.Add(new OrderHerb
                        {
                            HerbId = item.ItemId,
                            Quantity = item.Quantity,
                            UnitPrice = item.UnitPrice,
                            SubTotal = item.SubTotal,
                            SubOrder = subOrder
                        });
                    }
                    else if (item.ItemType == CartItemType.Recipe)
                    {
                        subOrder.OrderRecipes.Add(new OrderRecipe
                        {
                            RecipeId = item.ItemId,
                            Quantity = item.Quantity,
                            UnitPrice = item.UnitPrice,
                            SubTotal = item.SubTotal,
                            SubOrder = subOrder
                        });
                    }
                }

                order.SubOrders.Add(subOrder);
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            _cartService.ClearCart();
            return order.OrderId;
        }
    }
}