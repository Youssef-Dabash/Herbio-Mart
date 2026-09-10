using Microsoft.AspNetCore.Mvc;

namespace HerbioMart.Controllers;

public class CartController : Controller
{
    // عرض سلة المشتريات
    public IActionResult Index() => View();
}