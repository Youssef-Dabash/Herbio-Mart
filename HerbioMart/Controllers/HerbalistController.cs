using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HerbioMart.Controllers;


[Authorize(Roles = "Herbalist")]
public class HerbalistController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}