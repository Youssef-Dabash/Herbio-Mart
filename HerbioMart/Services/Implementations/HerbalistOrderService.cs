using Microsoft.EntityFrameworkCore;
using HerbioMart.Data;
using HerbioMart.Models.Enums;
using HerbioMart.Services.Interfaces;
using HerbioMart.ViewModels.Herbalist;

namespace HerbioMart.Services.Implementations
{
    public class HerbalistOrderService : IHerbalistOrderService
    {
        private readonly AppDbContext _context;

        public HerbalistOrderService(AppDbContext context)
        {
            _context = context;
        }

        private async Task<int?> GetHerbalistIdByUserIdAsync(int userId)
        {
            return await _context.Herbalists
                .Where(h => h.UserId == userId)
                .Select(h => (int?)h.HerbalistId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<HerbalistSubOrderListItemVM>> GetHerbalistSubOrdersAsync(int userId, SubOrderStatus? statusFilter = null)
        {
            var herbalistId = await GetHerbalistIdByUserIdAsync(userId);
            if (herbalistId == null) return new List<HerbalistSubOrderListItemVM>();

            var query = _context.SubOrders
                .Include(s => s.Order)
                    .ThenInclude(o => o.Patient)
                        .ThenInclude(p => p.User)
                .Include(s => s.OrderHerbs)
                .Include(s => s.OrderRecipes)
                .Where(s => s.HerbalistId == herbalistId.Value);

            if (statusFilter.HasValue)
            {
                query = query.Where(s => s.Status == statusFilter.Value);
            }

            return await query
                .OrderByDescending(s => s.Order.OrderDate)
                .Select(s => new HerbalistSubOrderListItemVM
                {
                    SubOrderId = s.SubOrderId,
                    OrderId = s.OrderId,
                    OrderDate = s.Order.OrderDate,
                    PatientName = s.Order.Patient.User.FullName,
                    TotalItemsCount = s.OrderHerbs.Count + s.OrderRecipes.Count,
                    SubTotal = s.SubTotal,
                    Status = s.Status
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<HerbalistSubOrderDetailsVM?> GetSubOrderDetailsAsync(int subOrderId, int userId)
        {
            var herbalistId = await GetHerbalistIdByUserIdAsync(userId);
            if (herbalistId == null) return null;

            var subOrder = await _context.SubOrders
                .Include(s => s.Order)
                    .ThenInclude(o => o.Patient)
                        .ThenInclude(p => p.User)
                .Include(s => s.OrderHerbs)
                    .ThenInclude(oh => oh.Herb)
                .Include(s => s.OrderRecipes)
                    .ThenInclude(or => or.Recipe)
                .FirstOrDefaultAsync(s => s.SubOrderId == subOrderId && s.HerbalistId == herbalistId.Value);

            if (subOrder == null) return null;
            var cleanAddress = ExtractCleanAddress(subOrder.Order.ShippingAddress);
            var vm = new HerbalistSubOrderDetailsVM
            {
                SubOrderId = subOrder.SubOrderId,
                OrderId = subOrder.OrderId,
                OrderDate = subOrder.Order.OrderDate,
                Status = subOrder.Status,
                SubTotal = subOrder.SubTotal,
                PatientName = subOrder.Order.Patient.User.FullName,
                PatientPhone = subOrder.Order.Patient.User.Phone ?? "Not Provided",
                ShippingAddress = subOrder.Order.ShippingAddress
            };

            foreach (var h in subOrder.OrderHerbs)
            {
                vm.Items.Add(new SubOrderItemDetailVM
                {
                    ItemName = h.Herb.HerbName,
                    ItemType = "Single Herb",
                    Quantity = h.Quantity,
                    UnitLabel = "g",
                    UnitPrice = h.UnitPrice,
                    SubTotal = h.SubTotal
                });
            }

            foreach (var r in subOrder.OrderRecipes)
            {
                vm.Items.Add(new SubOrderItemDetailVM
                {
                    ItemName = r.Recipe.RecipeName,
                    ItemType = "Remedy Pack",
                    Quantity = r.Quantity,
                    UnitLabel = "unit(s)",
                    UnitPrice = r.UnitPrice,
                    SubTotal = r.SubTotal
                });
            }

            return vm;
        }

        public async Task<bool> UpdateSubOrderStatusAsync(int subOrderId, SubOrderStatus newStatus, int userId)
        {
            var herbalistId = await GetHerbalistIdByUserIdAsync(userId);
            if (herbalistId == null) return false;

            var subOrder = await _context.SubOrders
                .Include(s => s.Order)
                    .ThenInclude(o => o.SubOrders)
                .FirstOrDefaultAsync(s => s.SubOrderId == subOrderId && s.HerbalistId == herbalistId.Value);

            if (subOrder == null) return false;

            if (newStatus == SubOrderStatus.Cancelled && subOrder.Status != SubOrderStatus.Pending)
            {
                return false;
            }

            if (subOrder.Status == SubOrderStatus.Shipped || subOrder.Status == SubOrderStatus.Cancelled)
            {
                return false;
            }

            subOrder.Status = newStatus;

            var parentOrder = subOrder.Order;
            var allSubOrders = parentOrder.SubOrders.ToList();

            if (allSubOrders.All(s => s.Status == SubOrderStatus.Cancelled))
            {
                parentOrder.OrderStatus = OrderStatus.Cancelled;
            }
            else if (allSubOrders.Where(s => s.Status != SubOrderStatus.Cancelled).All(s => s.Status == SubOrderStatus.Shipped))
            {
                parentOrder.OrderStatus = OrderStatus.Completed;
            }
            else if (allSubOrders.Any(s => s.Status == SubOrderStatus.Accepted || s.Status == SubOrderStatus.Shipped))
            {
                parentOrder.OrderStatus = OrderStatus.Processing;
            }
            else
            {
                parentOrder.OrderStatus = OrderStatus.Pending;
            }

            await _context.SaveChangesAsync();
            return true;
        }
        private static string ExtractCleanAddress(string? rawAddress)
        {
            if (string.IsNullOrWhiteSpace(rawAddress)) return "N/A";

            const string marker = "Delivery Address:";
            if (rawAddress.Contains(marker, StringComparison.OrdinalIgnoreCase))
            {
                var start = rawAddress.IndexOf(marker, StringComparison.OrdinalIgnoreCase) + marker.Length;
                var end = rawAddress.IndexOf("| Notes:", StringComparison.OrdinalIgnoreCase);

                if (end > start)
                    return rawAddress.Substring(start, end - start).Trim();

                return rawAddress.Substring(start).Trim();
            }

            return rawAddress.Trim();
        }
    }
}