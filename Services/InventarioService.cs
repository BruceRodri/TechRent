using Microsoft.AspNetCore.Identity;
using TechRent.Data;
using TechRent.Models;

namespace TechRent.Services
{
    public class InventarioService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public InventarioService(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public void RegistrarMovimiento(Equipo equipo, string tipoMovimiento, int cantidad, string? referencia = null, string? observacion = null, string? usuarioId = null)
        {
            var movimiento = new MovimientoInventario
            {
                EquipoId = equipo.Id,
                TipoMovimiento = tipoMovimiento,
                Cantidad = cantidad,
                StockAnterior = equipo.Stock + cantidad,
                StockPosterior = equipo.Stock,
                Referencia = referencia,
                FechaMovimiento = DateTime.UtcNow,
                UsuarioId = usuarioId,
                Observacion = observacion
            };
            _context.MovimientosInventario.Add(movimiento);
        }
    }
}
