using InteractiveMapGame.Services;

namespace InteractiveMapGame.Middleware
{
    public class AdminAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AdminAuthMiddleware> _logger;

        public AdminAuthMiddleware(RequestDelegate next, ILogger<AdminAuthMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IAuthService authService)
        {
            // Skip authentication for login page and API endpoints that don't require auth
            var path = context.Request.Path.Value?.ToLower() ?? "";
            
            _logger.LogInformation("Processing request for path: {Path}", path);
            
            if (path.StartsWith("/admin/login") || 
                path.StartsWith("/api/auth") ||
                path.StartsWith("/swagger") ||
                path.StartsWith("/_framework") ||
                path.StartsWith("/css") ||
                path.StartsWith("/js") ||
                path.StartsWith("/assets") ||
                path == "/" ||
                path == "/index.html")
            {
                _logger.LogInformation("Skipping authentication for path: {Path}", path);
                await _next(context);
                return;
            }

            // Check for admin session
            var sessionToken = GetSessionToken(context);
            _logger.LogInformation("Session token found: {HasToken}, Token: {Token}", !string.IsNullOrEmpty(sessionToken), sessionToken);
            
            if (string.IsNullOrEmpty(sessionToken))
            {
                if (path.StartsWith("/admin"))
                {
                    _logger.LogInformation("No session token, redirecting to login for path: {Path}", path);
                    context.Response.Redirect("/admin/login");
                    return;
                }
                
                await _next(context);
                return;
            }

            var admin = await authService.GetAdminFromSessionAsync(sessionToken);
            _logger.LogInformation("Admin found from session: {HasAdmin}", admin != null);
            
            if (admin == null)
            {
                if (path.StartsWith("/admin"))
                {
                    _logger.LogInformation("Invalid session, redirecting to login for path: {Path}", path);
                    context.Response.Redirect("/admin/login");
                    return;
                }
                
                await _next(context);
                return;
            }

            // Add admin to context for use in controllers
            context.Items["Admin"] = admin;
            _logger.LogInformation("Admin {Username} authenticated for path: {Path}", admin.Username, path);
            
            await _next(context);
        }

        private string? GetSessionToken(HttpContext context)
        {
            // Check for session token in cookie
            if (context.Request.Cookies.TryGetValue("AdminSession", out var cookieToken))
            {
                return cookieToken;
            }

            // Check for session token in Authorization header
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader?.StartsWith("Bearer ") == true)
            {
                return authHeader.Substring("Bearer ".Length).Trim();
            }

            return null;
        }
    }

    public static class AdminAuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseAdminAuth(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AdminAuthMiddleware>();
        }
    }
}
