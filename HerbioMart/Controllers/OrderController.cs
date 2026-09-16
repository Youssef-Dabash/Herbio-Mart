using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HerbioMart.Services.Interfaces;

namespace HerbioMart.Controllers
{
    [Authorize(Roles = "Patient")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // GET: /Order/Index (My Orders List)
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var orders = await _orderService.GetPatientOrdersAsync(CurrentUserId);
            return View(orders);
        }

        // GET: /Order/Details/{id} (Live Tracking Page)
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderDetailsAsync(id, CurrentUserId);
            if (order == null)
            {
                return NotFound("Order not found or you are unauthorized to view it.");
            }

            return View(order);
        }
    }
}