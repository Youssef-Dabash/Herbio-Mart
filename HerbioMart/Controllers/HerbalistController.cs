using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HerbioMart.Data;
using HerbioMart.ViewModels.Herbalist;

namespace HerbioMart.Controllers
{
    [Authorize(Roles = "Herbalist")]
    public class HerbalistController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public HerbalistController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ==========================================
        // 1. DASHBOARD OVERVIEW (INDEX)
        // ==========================================
        public async Task<IActionResult> Index()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId))
                return RedirectToAction("Login", "Account");

            var herbalist = await _context.Herbalists
                .Include(h => h.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.UserId == userId);

            if (herbalist == null)
                return NotFound("Herbalist record not found.");

            var addressParts = new[] { herbalist.User.Street, herbalist.User.City, herbalist.User.Governorate }
                .Where(s => !string.IsNullOrWhiteSpace(s));
            var fullAddress = string.Join(", ", addressParts);

            var model = new HerbalistDashboardVM
            {
                HerbalistId = herbalist.HerbalistId,
                FullName = herbalist.User.FullName,
                UserName = herbalist.User.UserName,
                Email = herbalist.User.Email,
                Phone = herbalist.User.Phone,
                FullAddress = fullAddress,
                ImageUrl = herbalist.User.ImageUrl,
                LicenseNumber = herbalist.LicenseNumber,
                Bio = herbalist.Bio,

                AddedHerbsCount = await _context.Herbs.CountAsync(h => h.AddedByHerbalistId == herbalist.HerbalistId),
                FormulatedRecipesCount = await _context.Recipes.CountAsync(r => r.HerbalistId == herbalist.HerbalistId),
                PendingOrdersCount = await _context.SubOrders.CountAsync(s => s.HerbalistId == herbalist.HerbalistId && s.Status.ToString() == "Pending")
            };

            return View(model);
        }

        // ==========================================
        // 2. EDIT PROFILE (GET)
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId))
                return RedirectToAction("Login", "Account");

            var herbalist = await _context.Herbalists
                .Include(h => h.User)
                .FirstOrDefaultAsync(h => h.UserId == userId);

            if (herbalist == null)
                return NotFound();

            var model = new EditHerbalistProfileVM
            {
                FullName = herbalist.User.FullName,
                Phone = herbalist.User.Phone,
                Governorate = herbalist.User.Governorate,
                City = herbalist.User.City,
                Street = herbalist.User.Street,
                LicenseNumber = herbalist.LicenseNumber,
                Bio = herbalist.Bio,
                ExistingImageUrl = herbalist.User.ImageUrl
            };

            return View(model);
        }

        // ==========================================
        // 3. EDIT PROFILE (POST)
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EditHerbalistProfileVM model)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId))
                return RedirectToAction("Login", "Account");

            var herbalist = await _context.Herbalists
                .Include(h => h.User)
                .FirstOrDefaultAsync(h => h.UserId == userId);

            if (herbalist == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                model.ExistingImageUrl = herbalist.User.ImageUrl;
                return View(model);
            }

            if (model.ProfileImage != null && model.ProfileImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "users");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                if (!string.IsNullOrEmpty(herbalist.User.ImageUrl))
                {
                    var oldFilePath = Path.Combine(_env.WebRootPath, herbalist.User.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        try { System.IO.File.Delete(oldFilePath); } catch { /* Ignore if in use */ }
                    }
                }

                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(model.ProfileImage.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfileImage.CopyToAsync(fileStream);
                }

                herbalist.User.ImageUrl = $"/uploads/users/{uniqueFileName}";
            }

            herbalist.User.FullName = model.FullName;
            herbalist.User.Phone = model.Phone;
            herbalist.User.Governorate = model.Governorate ?? string.Empty;
            herbalist.User.City = model.City ?? string.Empty;
            herbalist.User.Street = model.Street ?? string.Empty;

            herbalist.LicenseNumber = model.LicenseNumber;
            herbalist.Bio = model.Bio;

            await _context.SaveChangesAsync();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, herbalist.User.Id.ToString()),
                new Claim(ClaimTypes.Name, herbalist.User.UserName),
                new Claim("FullName", herbalist.User.FullName),
                new Claim(ClaimTypes.Role, herbalist.User.Role.ToString()),
                new Claim("ProfilePicture", herbalist.User.ImageUrl ?? "")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            TempData["SuccessMessage"] = "Profile details updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // 4. INCOMING ORDERS (SUB-ORDERS)
        // ==========================================
        public async Task<IActionResult> Orders()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId))
                return RedirectToAction("Login", "Account");

            var herbalist = await _context.Herbalists
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.UserId == userId);

            if (herbalist == null)
                return NotFound();

            var orders = await _context.SubOrders
                .Include(s => s.Order)
                //.Include(s => s.SubOrder)
                .Where(s => s.HerbalistId == herbalist.HerbalistId)
                .OrderByDescending(s => s.SubOrderId)
                .AsNoTracking()
                .ToListAsync();

            return View(orders);
        }
    }
}