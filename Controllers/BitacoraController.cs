using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechRent.Data;

namespace TechRent.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class BitacoraController : Controller
    {
        private readonly AppDbContext _context;

        public BitacoraController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(
            int pageNumber = 1,
            string? accion = null,
            string? email = null,
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null)
        {
            int pageSize = 25;

            var query = _context.AuditLogs
                .AsNoTracking()
                .OrderByDescending(a => a.Fecha)
                .AsQueryable();

            if (!string.IsNullOrEmpty(accion))
                query = query.Where(a => a.Accion.Contains(accion));

            if (!string.IsNullOrEmpty(email))
                query = query.Where(a => a.Email != null && a.Email.Contains(email));

            if (fechaDesde.HasValue)
                query = query.Where(a => a.Fecha >= fechaDesde.Value);

            if (fechaHasta.HasValue)
                query = query.Where(a => a.Fecha <= fechaHasta.Value.AddDays(1));

            var totalRegistros = await query.CountAsync();

            var registros = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.TotalRegistros = totalRegistros;
            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRegistros / pageSize);
            ViewBag.Accion = accion;
            ViewBag.Email = email;
            ViewBag.FechaDesde = fechaDesde?.ToString("yyyy-MM-dd");
            ViewBag.FechaHasta = fechaHasta?.ToString("yyyy-MM-dd");

            return View(registros);
        }
    }
}
