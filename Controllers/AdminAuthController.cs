using Microsoft.AspNetCore.Mvc;
using InteractiveMapGame.Services;
using InteractiveMapGame.Models;
using System.ComponentModel.DataAnnotations;

namespace InteractiveMapGame.Controllers
{
    [Route("admin")]
    public class AdminAuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AdminAuthController> _logger;

        public AdminAuthController(IAuthService authService, ILogger<AdminAuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            // Check if already logged in
            if (HttpContext.Items.ContainsKey("Admin"))
            {
                return RedirectToAction("Dashboard", "Admin");
            }

            return View();
        }

        [HttpGet("test-login")]
        public IActionResult TestLogin()
        {
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            _logger.LogInformation("Login attempt for username: {Username}", model.Username);
            
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state is invalid for username: {Username}", model.Username);
                return View(model);
            }

            try
            {
                var admin = await _authService.AuthenticateAsync(model.Username, model.Password);
                
                if (admin == null)
                {
                    _logger.LogWarning("Authentication failed for username: {Username}", model.Username);
                    ModelState.AddModelError("", "Invalid username or password");
                    return View(model);
                }

                // Create session
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
                var userAgent = HttpContext.Request.Headers["User-Agent"].FirstOrDefault();
                var sessionToken = await _authService.CreateSessionAsync(admin, ipAddress, userAgent);

                _logger.LogInformation("Created session token: {SessionToken}", sessionToken);

                // Set session cookie with simpler options
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.UtcNow.AddDays(7),
                    Path = "/"
                };
                
                Response.Cookies.Append("AdminSession", sessionToken, cookieOptions);

                _logger.LogInformation("Admin {Username} logged in successfully with session {SessionToken}", admin.Username, sessionToken);
                
                // Test the session immediately
                var testAdmin = await _authService.GetAdminFromSessionAsync(sessionToken);
                _logger.LogInformation("Session test result: {TestResult}", testAdmin != null ? "SUCCESS" : "FAILED");
                
                // Force a redirect to ensure the response is sent
                return Redirect("/admin/dashboard");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during admin login for {Username}", model.Username);
                ModelState.AddModelError("", "An error occurred during login. Please try again.");
                return View(model);
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var sessionToken = Request.Cookies["AdminSession"];
            
            if (!string.IsNullOrEmpty(sessionToken))
            {
                await _authService.LogoutAsync(sessionToken);
            }

            Response.Cookies.Delete("AdminSession");
            
            return RedirectToAction("Login");
        }

        [HttpGet("logout")]
        public async Task<IActionResult> LogoutGet()
        {
            var sessionToken = Request.Cookies["AdminSession"];
            
            if (!string.IsNullOrEmpty(sessionToken))
            {
                await _authService.LogoutAsync(sessionToken);
            }

            Response.Cookies.Delete("AdminSession");
            
            return RedirectToAction("Login");
        }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}
