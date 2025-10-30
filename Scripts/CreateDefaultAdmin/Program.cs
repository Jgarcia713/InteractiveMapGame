using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using InteractiveMapGame.Data;
using InteractiveMapGame.Models;
using InteractiveMapGame.Services;

// Create a default admin user
var options = new DbContextOptionsBuilder<MapGameDbContext>()
    .UseSqlite("Data Source=../../mapgame.db")
    .Options;

using var context = new MapGameDbContext(options);
var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
var authService = new AuthService(context, loggerFactory.CreateLogger<AuthService>());

// Check if any admins exist
if (await context.Admins.AnyAsync())
{
    Console.WriteLine("Admin users already exist. Skipping default admin creation.");
    return;
}

// Create default admin
var defaultAdmin = new Admin
{
    Username = "admin",
    Email = "admin@example.com",
    FullName = "System Administrator",
    PasswordHash = authService.HashPassword("admin123"),
    IsActive = true,
    IsSuperAdmin = true,
    CreatedAt = DateTime.UtcNow,
    UpdatedAt = DateTime.UtcNow
};

context.Admins.Add(defaultAdmin);
await context.SaveChangesAsync();

Console.WriteLine("Default admin created successfully!");
Console.WriteLine("Username: admin");
Console.WriteLine("Password: admin123");
Console.WriteLine("Please change the password after first login.");
