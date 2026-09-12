using Microsoft.AspNetCore.Mvc;

namespace HerbioMart.Controllers;

public class ShopController : Controller
{
    // عرض كاتالوج الأعشاب والوصفات
    public IActionResult Index() => View();

    // تفاصيل العشب أو الوصفة
    public IActionResult Details(int? id) => View();
}