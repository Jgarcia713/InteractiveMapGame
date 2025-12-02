using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using InteractiveMapGame.Data;
using InteractiveMapGame.Models;

namespace InteractiveMapGame.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminSeedController : ControllerBase
    {
        private readonly MapGameDbContext _context;

        public AdminSeedController(MapGameDbContext context)
        {
            _context = context;
        }

        // POST: api/AdminSeed/create-initial-admin
        // This endpoint should only be used once to create the first admin
        [HttpPost("create-initial-admin")]
        public async Task<ActionResult> CreateInitialAdmin([FromBody] CreateAdminRequest request)
        {
            // Check if any admin already exists
            if (await _context.Admins.AnyAsync())
            {
                return BadRequest(new { message = "An admin already exists. Use the admin panel to create additional admins." });
            }

            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { message = "Username and password are required" });
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

            return Ok(new { 
                message = "Initial admin created successfully!",
                username = admin.Username
            });
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}

