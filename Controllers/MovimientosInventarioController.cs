using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechRent.Data;

namespace TechRent.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class MovimientosInventarioController : Controller
    {
        private readonly AppDbContext _context;

        public MovimientosInventarioController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, int? equipoId = null, string? tipoMovimiento = null)
        {
            int pageSize = 20;

            var query = _context.MovimientosInventario
                .Include(m => m.Equipo)
                .AsNoTracking()
                .OrderByDescending(m => m.FechaMovimiento)
                .AsQueryable();

            if (equipoId.HasValue)
                query = query.Where(m => m.EquipoId == equipoId.Value);

            if (!string.IsNullOrEmpty(tipoMovimiento))
                query = query.Where(m => m.TipoMovimiento == tipoMovimiento);

            var totalRegistros = await query.CountAsync();

            var movimientos = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.TotalRegistros = totalRegistros;
            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRegistros / pageSize);
            ViewBag.EquipoId = equipoId;
            ViewBag.TipoMovimiento = tipoMovimiento;

            ViewData["Equipos"] = await _context.Equipos
                .Where(e => e.Activo)
                .OrderBy(e => e.Nombre)
                .Select(e => new { e.Id, e.Nombre })
                .ToListAsync();

            return View(movimientos);
        }
    }
}
