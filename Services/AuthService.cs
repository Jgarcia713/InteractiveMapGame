using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using InteractiveMapGame.Data;
using InteractiveMapGame.Models;

namespace InteractiveMapGame.Services
{
    public interface IAuthService
    {
        Task<Admin?> AuthenticateAsync(string username, string password);
        Task<string> CreateSessionAsync(Admin admin, string ipAddress, string? userAgent);
        Task<Admin?> GetAdminFromSessionAsync(string sessionToken);
        Task<bool> ValidateSessionAsync(string sessionToken);
        Task LogoutAsync(string sessionToken);
        Task LogoutAllSessionsAsync(int adminId);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }

    public class AuthService : IAuthService
    {
        private readonly MapGameDbContext _context;
        private readonly ILogger<AuthService> _logger;

        public AuthService(MapGameDbContext context, ILogger<AuthService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Admin?> AuthenticateAsync(string username, string password)
        {
            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.Username == username && a.IsActive);

            if (admin == null || !VerifyPassword(password, admin.PasswordHash))
            {
                return null;
            }

            // Update last login
            admin.LastLoginAt = DateTime.UtcNow;
            admin.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return admin;
        }

        public async Task<string> CreateSessionAsync(Admin admin, string ipAddress, string? userAgent)
        {
            var sessionToken = GenerateSessionToken();
            var expiresAt = DateTime.UtcNow.AddDays(7); // 7 day session

            var session = new AdminSession
            {
                AdminId = admin.Id,
                SessionToken = sessionToken,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                ExpiresAt = expiresAt,
                LastActivityAt = DateTime.UtcNow
            };

            _context.AdminSessions.Add(session);
            await _context.SaveChangesAsync();

            return sessionToken;
        }

        public async Task<Admin?> GetAdminFromSessionAsync(string sessionToken)
        {
            var now = DateTime.UtcNow;
            _logger.LogInformation("Checking session {SessionToken} against time {Now}", sessionToken, now);
            
            var session = await _context.AdminSessions
                .Include(s => s.Admin)
                .FirstOrDefaultAsync(s => s.SessionToken == sessionToken && 
                                        s.IsActive);

            if (session == null)
            {
                _logger.LogWarning("Session not found or inactive: {SessionToken}", sessionToken);
                return null;
            }

            _logger.LogInformation("Session found, expires at {ExpiresAt}, current time {Now}", session.ExpiresAt, now);
            
            if (session.ExpiresAt <= now)
            {
                _logger.LogWarning("Session expired: {ExpiresAt} <= {Now}", session.ExpiresAt, now);
                return null;
            }

            // Update last activity
            session.LastActivityAt = now;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Session valid, returning admin {AdminId}", session.AdminId);
            return session.Admin;
        }

        public async Task<bool> ValidateSessionAsync(string sessionToken)
        {
            var session = await _context.AdminSessions
                .FirstOrDefaultAsync(s => s.SessionToken == sessionToken && 
                                        s.IsActive && 
                                        s.ExpiresAt > DateTime.UtcNow);

            return session != null;
        }

        public async Task LogoutAsync(string sessionToken)
        {
            var session = await _context.AdminSessions
                .FirstOrDefaultAsync(s => s.SessionToken == sessionToken);

            if (session != null)
            {
                session.IsActive = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task LogoutAllSessionsAsync(int adminId)
        {
            var sessions = await _context.AdminSessions
                .Where(s => s.AdminId == adminId && s.IsActive)
                .ToListAsync();

            foreach (var session in sessions)
            {
                session.IsActive = false;
            }

            await _context.SaveChangesAsync();
        }

        public string HashPassword(string password)
        {
            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[32];
            rng.GetBytes(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(32);

            var combined = new byte[64];
            Array.Copy(salt, 0, combined, 0, 32);
            Array.Copy(hash, 0, combined, 32, 32);

            return Convert.ToBase64String(combined);
        }

        public bool VerifyPassword(string password, string hash)
        {
            try
            {
                var combined = Convert.FromBase64String(hash);
                var salt = new byte[32];
                var storedHash = new byte[32];

                Array.Copy(combined, 0, salt, 0, 32);
                Array.Copy(combined, 32, storedHash, 0, 32);

                using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
                var computedHash = pbkdf2.GetBytes(32);

                return computedHash.SequenceEqual(storedHash);
            }
            catch
            {
                return false;
            }
        }

        private string GenerateSessionToken()
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[32];
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
        }
    }
}
