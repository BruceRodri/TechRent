using System.Security.Claims;
using TechRent.Data;
using TechRent.Models;

namespace TechRent.Services
{
    public class AuditService : IAuditService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AuditService> _logger;

        public AuditService(IServiceProvider serviceProvider, ILogger<AuditService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task RegistrarAsync(string accion, string? detalles = null, ClaimsPrincipal? user = null, HttpContext? httpContext = null)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var userId = user?.FindFirstValue(ClaimTypes.NameIdentifier);
                var email = user?.FindFirstValue(ClaimTypes.Email) ?? user?.Identity?.Name;

                var log = new AuditLog
                {
                    UserId = userId,
                    Email = email,
                    Accion = accion,
                    Metodo = httpContext?.Request?.Method,
                    Ruta = httpContext?.Request?.Path,
                    IpAddress = GetClientIp(httpContext),
                    UserAgent = httpContext?.Request?.Headers.UserAgent.ToString(),
                    Detalles = detalles,
                    Fecha = DateTime.UtcNow
                };

                context.AuditLogs.Add(log);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error al registrar auditoria: {Accion}", accion);
            }
        }

        private static string? GetClientIp(HttpContext? httpContext)
        {
            if (httpContext == null) return null;
            return httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                ?? httpContext.Connection.RemoteIpAddress?.ToString();
        }
    }
}
