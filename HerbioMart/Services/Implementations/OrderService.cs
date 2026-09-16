using Microsoft.EntityFrameworkCore;
using HerbioMart.Data;
using HerbioMart.Models.Entities;
using HerbioMart.Services.Interfaces;
using HerbioMart.ViewModels.Shop;

namespace HerbioMart.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HerbVendorOptionVM>> GetAvailableVendorsForHerbAsync(int herbId)
        {
            return await _context.HerbalistHerbs
                .Where(hh => hh.HerbId == herbId && hh.IsActive)
                .Include(hh => hh.Herbalist)
                    .ThenInclude(h => h.User)
                .Select(hh => new HerbVendorOptionVM
                {
                    HerbalistId = hh.HerbalistId,
                    HerbalistName = hh.Herbalist.User.FullName,
                    Price = hh.Price
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Order?> GetOrderDetailsAsync(int orderId, int userId)
        {
            return await _context.Orders
                .Include(o => o.SubOrders)
                    .ThenInclude(s => s.Herbalist)
                        .ThenInclude(h => h.User)
                .Include(o => o.SubOrders)
                    .ThenInclude(s => s.OrderHerbs)
                        .ThenInclude(oh => oh.Herb)
                .Include(o => o.SubOrders)
                    .ThenInclude(s => s.OrderRecipes)
                        .ThenInclude(or => or.Recipe)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.Patient.UserId == userId);
        }

        public async Task<IEnumerable<Order>> GetPatientOrdersAsync(int userId)
        {
            return await _context.Orders
                .Include(o => o.SubOrders)
                .Where(o => o.Patient.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}