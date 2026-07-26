using System.Security.Claims;
using TechRent.Services;

namespace TechRent.Middleware
{
    public class AuditMiddleware
    {
        private readonly RequestDelegate _next;

        public AuditMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            var path = context.Request.Path.Value ?? "";
            var method = context.Request.Method;
            var statusCode = context.Response.StatusCode;

            if (path.StartsWith("/Identity/Account/Login", StringComparison.OrdinalIgnoreCase)
                && method == "POST"
                && statusCode >= 200 && statusCode < 400)
            {
                var auditService = context.RequestServices.GetRequiredService<IAuditService>();
                await auditService.RegistrarAsync(
                    "Inicio de sesion exitoso",
                    $"Ruta destino: {context.Request.Query["ReturnUrl"].FirstOrDefault() ?? "/"}",
                    context.User,
                    context);
            }

            if (path.StartsWith("/Identity/Account/Logout", StringComparison.OrdinalIgnoreCase)
                && method == "POST")
            {
                var auditService = context.RequestServices.GetRequiredService<IAuditService>();
                await auditService.RegistrarAsync("Cierre de sesion", null, context.User, context);
            }

            if (path.Contains("/AccessDenied", StringComparison.OrdinalIgnoreCase))
            {
                var auditService = context.RequestServices.GetRequiredService<IAuditService>();
                await auditService.RegistrarAsync(
                    "Acceso denegado",
                    $"Ruta: {context.Request.Path}",
                    context.User,
                    context);
            }

            if (path.StartsWith("/Account/LoginWith2fa", StringComparison.OrdinalIgnoreCase)
                && method == "POST"
                && statusCode >= 200 && statusCode < 400)
            {
                var auditService = context.RequestServices.GetRequiredService<IAuditService>();
                await auditService.RegistrarAsync(
                    "Verificacion 2FA exitosa",
                    null,
                    context.User,
                    context);
            }

            if (path.StartsWith("/Identity/Account/Lockout", StringComparison.OrdinalIgnoreCase))
            {
                var auditService = context.RequestServices.GetRequiredService<IAuditService>();
                await auditService.RegistrarAsync(
                    "Cuenta bloqueada por intentos fallidos",
                    $"Email: {context.User?.Identity?.Name}",
                    context.User,
                    context);
            }
        }
    }
}
