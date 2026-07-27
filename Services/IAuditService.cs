using System.Security.Claims;

namespace TechRent.Services
{
    public interface IAuditService
    {
        Task RegistrarAsync(string accion, string? detalles = null, ClaimsPrincipal? user = null, HttpContext? httpContext = null);

        Task RegistrarAsync(
            string accion,
            string? entidad = null,
            string? identificadorEntidad = null,
            string? valorAnterior = null,
            string? valorNuevo = null,
            string? detalles = null,
            ClaimsPrincipal? user = null,
            HttpContext? httpContext = null);
    }
}
