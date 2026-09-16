using HerbioMart.Data;
using HerbioMart.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HerbioMart.ViewComponents
{
    public class SiteStatsViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public SiteStatsViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var stats = new SiteStatsVM
            {
                PatientsCount = await _context.Patients.AsNoTracking().CountAsync(),
                HerbalistsCount = await _context.Herbalists.AsNoTracking().CountAsync(),
                HerbsCount = await _context.Herbs.AsNoTracking().CountAsync(),
                RecipesCount = await _context.Recipes.AsNoTracking().Where(r => r.IsActive).CountAsync(),
                OrdersCount = await _context.SubOrders.AsNoTracking().CountAsync(),
                FeedbacksCount = await _context.Feedbacks.AsNoTracking().CountAsync()
            };

            return View("~/Views/Shared/_SiteStats.cshtml", stats);
        }
    }
}