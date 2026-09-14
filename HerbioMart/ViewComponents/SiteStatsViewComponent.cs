using HerbioMart.Data;
using HerbioMart.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HerbioMart.ViewComponents;

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
            FeedbacksCount = await _context.Feedbacks.AsNoTracking().CountAsync(),
            OrdersCount = await _context.SubOrders.AsNoTracking().CountAsync()
        };

        // Direct path to your view file in Views/Shared
        return View("~/Views/Shared/_SiteStats.cshtml", stats);
    }
}