using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using InteractiveMapGame.Data;
using InteractiveMapGame.Models;

namespace InteractiveMapGame.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly MapGameDbContext _context;

        public AdminController(MapGameDbContext context)
        {
            _context = context;
        }

        // POST: api/Admin/login
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { message = "Username and password are required" });
            }

            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.Username == request.Username && a.IsActive);

            if (admin == null || !VerifyPassword(request.Password, admin.PasswordHash))
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            // Update last login
            admin.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Create claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, admin.Username),
                new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()),
                new Claim("AdminId", admin.Id.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = request.RememberMe,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return Ok(new { message = "Login successful", username = admin.Username });
        }

        // POST: api/Admin/logout
        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { message = "Logout successful" });
        }

        // GET: api/Admin/check-auth
        [HttpGet("check-auth")]
        public ActionResult CheckAuth()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return Ok(new { 
                    isAuthenticated = true, 
                    username = User.Identity.Name 
                });
            }
            return Ok(new { isAuthenticated = false });
        }

        // GET: api/Admin/list
        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<AdminDto>>> GetAdmins()
        {
            if (!User.Identity?.IsAuthenticated == true)
            {
                return Unauthorized(new { message = "Authentication required" });
            }

            var admins = await _context.Admins
                .Select(a => new AdminDto
                {
                    Id = a.Id,
                    Username = a.Username,
                    Email = a.Email,
                    IsActive = a.IsActive,
                    CreatedAt = a.CreatedAt,
                    LastLoginAt = a.LastLoginAt
                })
                .ToListAsync();

            return Ok(admins);
        }

        // POST: api/Admin/create
        [HttpPost("create")]
        public async Task<ActionResult<AdminDto>> CreateAdmin([FromBody] CreateAdminRequest request)
        {
            // Allow creating first admin without authentication
            var hasAnyAdmin = await _context.Admins.AnyAsync();
            if (hasAnyAdmin && !User.Identity?.IsAuthenticated == true)
            {
                return Unauthorized(new { message = "Authentication required" });
            }

            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { message = "Username and password are required" });
            }

            // Check if username already exists
            if (await _context.Admins.AnyAsync(a => a.Username == request.Username))
            {
                return Conflict(new { message = "Username already exists" });
            }

            var admin = new Admin
            {
                Username = request.Username,
                PasswordHash = HashPassword(request.Password),
                Email = request.Email,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();

            return Ok(new AdminDto
            {
                Id = admin.Id,
                Username = admin.Username,
                Email = admin.Email,
                IsActive = admin.IsActive,
                CreatedAt = admin.CreatedAt,
                LastLoginAt = admin.LastLoginAt
            });
        }

        // DELETE: api/Admin/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAdmin(int id)
        {
            if (!User.Identity?.IsAuthenticated == true)
            {
                return Unauthorized(new { message = "Authentication required" });
            }

            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                return NotFound(new { message = "Admin not found" });
            }

            // Prevent deleting yourself
            var currentAdminId = int.Parse(User.FindFirst("AdminId")?.Value ?? "0");
            if (admin.Id == currentAdminId)
            {
                return BadRequest(new { message = "Cannot delete your own account" });
            }

            _context.Admins.Remove(admin);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Admin deleted successfully" });
        }

        // PUT: api/Admin/{id}/deactivate
        [HttpPut("{id}/deactivate")]
        public async Task<ActionResult> DeactivateAdmin(int id)
        {
            if (!User.Identity?.IsAuthenticated == true)
            {
                return Unauthorized(new { message = "Authentication required" });
            }

            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                return NotFound(new { message = "Admin not found" });
            }

            // Prevent deactivating yourself
            var currentAdminId = int.Parse(User.FindFirst("AdminId")?.Value ?? "0");
            if (admin.Id == currentAdminId)
            {
                return BadRequest(new { message = "Cannot deactivate your own account" });
            }

            admin.IsActive = false;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Admin deactivated successfully" });
        }

        // PUT: api/Admin/{id}/activate
        [HttpPut("{id}/activate")]
        public async Task<ActionResult> ActivateAdmin(int id)
        {
            if (!User.Identity?.IsAuthenticated == true)
            {
                return Unauthorized(new { message = "Authentication required" });
            }

            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                return NotFound(new { message = "Admin not found" });
            }

            admin.IsActive = true;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Admin activated successfully" });
        }

        // PUT: api/Admin/{id}/change-password
        [HttpPut("{id}/change-password")]
        public async Task<ActionResult> ChangePassword(int id, [FromBody] ChangePasswordRequest request)
        {
            if (!User.Identity?.IsAuthenticated == true)
            {
                return Unauthorized(new { message = "Authentication required" });
            }

            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                return NotFound(new { message = "Admin not found" });
            }

            // Only allow changing own password unless it's a different admin
            var currentAdminId = int.Parse(User.FindFirst("AdminId")?.Value ?? "0");
            if (admin.Id != currentAdminId && string.IsNullOrEmpty(request.OldPassword))
            {
                // Admin changing another admin's password - no old password needed
                admin.PasswordHash = HashPassword(request.NewPassword);
            }
            else if (admin.Id == currentAdminId)
            {
                // Changing own password - verify old password
                if (string.IsNullOrEmpty(request.OldPassword) || !VerifyPassword(request.OldPassword, admin.PasswordHash))
                {
                    return BadRequest(new { message = "Invalid old password" });
                }
                admin.PasswordHash = HashPassword(request.NewPassword);
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Password changed successfully" });
        }

        // Helper methods for password hashing
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string password, string hash)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput == hash;
        }
    }

    public record LoginRequest(string Username, string Password, bool RememberMe = false);
    public record CreateAdminRequest(string Username, string Password, string? Email = null);
    public record ChangePasswordRequest(string? OldPassword, string NewPassword);
    public class AdminDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? Email { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }
}

