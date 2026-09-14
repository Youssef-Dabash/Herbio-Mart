using HerbioMart.Models;
using HerbioMart.ViewModels.Home;
using HerbioMart.ViewModels.Shop;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace HerbioMart.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();
    public IActionResult Contact() => View();

    public IActionResult Testimonials() => View();

    public IActionResult Privacy() => View();

    public IActionResult NotFoundPage() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
