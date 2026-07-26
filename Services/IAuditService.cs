using System.Security.Claims;

namespace TechRent.Services
{
    public interface IAuditService
    {
        Task RegistrarAsync(string accion, string? detalles = null, ClaimsPrincipal? user = null, HttpContext? httpContext = null);
    }
}
