using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HerbioMart.Controllers;

[Authorize(Roles = "Patient")]
public class PatientController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}