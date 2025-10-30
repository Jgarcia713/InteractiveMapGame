using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InteractiveMapGame.Data;
using InteractiveMapGame.Models;
using InteractiveMapGame.Services;
using System.ComponentModel.DataAnnotations;

namespace InteractiveMapGame.Controllers
{
    [Route("admin")]
    public class AdminController : Controller
    {
        private readonly MapGameDbContext _context;
        private readonly IAuthService _authService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(MapGameDbContext context, IAuthService authService, ILogger<AdminController> logger)
        {
            _context = context;
            _authService = authService;
            _logger = logger;
        }

        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            var admin = HttpContext.Items["Admin"] as Admin;
            if (admin == null)
            {
                return RedirectToAction("Login", "AdminAuth");
            }

            ViewBag.Admin = admin;
            return View();
        }

        [HttpGet("analytics")]
        public IActionResult Analytics()
        {
            var admin = HttpContext.Items["Admin"] as Admin;
            if (admin == null)
            {
                return RedirectToAction("Login", "AdminAuth");
            }

            ViewBag.Admin = admin;
            return View();
        }

        [HttpGet("content")]
        public IActionResult Content()
        {
            var admin = HttpContext.Items["Admin"] as Admin;
            if (admin == null)
            {
                return RedirectToAction("Login", "AdminAuth");
            }

            ViewBag.Admin = admin;
            return View();
        }

        [HttpGet("map")]
        public IActionResult Map()
        {
            var admin = HttpContext.Items["Admin"] as Admin;
            if (admin == null)
            {
                return RedirectToAction("Login", "AdminAuth");
            }

            ViewBag.Admin = admin;
            return View();
        }

        [HttpGet("admins")]
        public async Task<IActionResult> Admins()
        {
            var admin = HttpContext.Items["Admin"] as Admin;
            if (admin == null)
            {
                return RedirectToAction("Login", "AdminAuth");
            }

            var admins = await _context.Admins
                .OrderBy(a => a.Username)
                .ToListAsync();

            ViewBag.Admin = admin;
            ViewBag.Admins = admins;
            return View();
        }

        [HttpGet("admins/create")]
        public IActionResult CreateAdmin()
        {
            var admin = HttpContext.Items["Admin"] as Admin;
            if (admin == null)
            {
                return RedirectToAction("Login", "AdminAuth");
            }

            ViewBag.Admin = admin;
            return View(new CreateAdminViewModel());
        }

        [HttpPost("admins/create")]
        public async Task<IActionResult> CreateAdmin(CreateAdminViewModel model)
        {
            var currentAdmin = HttpContext.Items["Admin"] as Admin;
            if (currentAdmin == null)
            {
                return RedirectToAction("Login", "AdminAuth");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Admin = currentAdmin;
                return View(model);
            }

            try
            {
                // Check if username or email already exists
                var existingAdmin = await _context.Admins
                    .FirstOrDefaultAsync(a => a.Username == model.Username || a.Email == model.Email);

                if (existingAdmin != null)
                {
                    if (existingAdmin.Username == model.Username)
                        ModelState.AddModelError("Username", "Username already exists");
                    if (existingAdmin.Email == model.Email)
                        ModelState.AddModelError("Email", "Email already exists");
                    
                    ViewBag.Admin = currentAdmin;
                    return View(model);
                }

                var newAdmin = new Admin
                {
                    Username = model.Username,
                    Email = model.Email,
                    FullName = model.FullName,
                    PasswordHash = _authService.HashPassword(model.Password),
                    IsActive = true,
                    IsSuperAdmin = model.IsSuperAdmin,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Admins.Add(newAdmin);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Admin {CurrentAdmin} created new admin {NewAdmin}", 
                    currentAdmin.Username, newAdmin.Username);

                TempData["SuccessMessage"] = "Admin created successfully";
                return RedirectToAction("Admins");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating admin {Username}", model.Username);
                ModelState.AddModelError("", "An error occurred while creating the admin. Please try again.");
                ViewBag.Admin = currentAdmin;
                return View(model);
            }
        }

        [HttpPost("admins/{id}/delete")]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            var currentAdmin = HttpContext.Items["Admin"] as Admin;
            if (currentAdmin == null)
            {
                return RedirectToAction("Login", "AdminAuth");
            }

            try
            {
                var admin = await _context.Admins.FindAsync(id);
                if (admin == null)
                {
                    TempData["ErrorMessage"] = "Admin not found";
                    return RedirectToAction("Admins");
                }

                // Prevent deleting self
                if (admin.Id == currentAdmin.Id)
                {
                    TempData["ErrorMessage"] = "You cannot delete your own account";
                    return RedirectToAction("Admins");
                }

                // Prevent non-super admins from deleting super admins
                if (admin.IsSuperAdmin && !currentAdmin.IsSuperAdmin)
                {
                    TempData["ErrorMessage"] = "You do not have permission to delete super admins";
                    return RedirectToAction("Admins");
                }

                _context.Admins.Remove(admin);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Admin {CurrentAdmin} deleted admin {DeletedAdmin}", 
                    currentAdmin.Username, admin.Username);

                TempData["SuccessMessage"] = "Admin deleted successfully";
                return RedirectToAction("Admins");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting admin {Id}", id);
                TempData["ErrorMessage"] = "An error occurred while deleting the admin. Please try again.";
                return RedirectToAction("Admins");
            }
        }

        [HttpPost("admins/{id}/toggle-status")]
        public async Task<IActionResult> ToggleAdminStatus(int id)
        {
            var currentAdmin = HttpContext.Items["Admin"] as Admin;
            if (currentAdmin == null)
            {
                return RedirectToAction("Login", "AdminAuth");
            }

            try
            {
                var admin = await _context.Admins.FindAsync(id);
                if (admin == null)
                {
                    TempData["ErrorMessage"] = "Admin not found";
                    return RedirectToAction("Admins");
                }

                // Prevent toggling self
                if (admin.Id == currentAdmin.Id)
                {
                    TempData["ErrorMessage"] = "You cannot deactivate your own account";
                    return RedirectToAction("Admins");
                }

                admin.IsActive = !admin.IsActive;
                admin.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                var status = admin.IsActive ? "activated" : "deactivated";
                _logger.LogInformation("Admin {CurrentAdmin} {Status} admin {TargetAdmin}", 
                    currentAdmin.Username, status, admin.Username);

                TempData["SuccessMessage"] = $"Admin {status} successfully";
                return RedirectToAction("Admins");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling admin status {Id}", id);
                TempData["ErrorMessage"] = "An error occurred while updating the admin status. Please try again.";
                return RedirectToAction("Admins");
            }
        }
    }

    public class CreateAdminViewModel
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string? FullName { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Display(Name = "Super Admin")]
        public bool IsSuperAdmin { get; set; }
    }
}
