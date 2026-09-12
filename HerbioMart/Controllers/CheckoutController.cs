using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HerbioMart.Controllers;

//[Authorize(Roles = "Patient")]
public class CheckoutController : Controller
{
    // إتمام الطلب وتأكيد العنوان
    public IActionResult Index() => View();
}