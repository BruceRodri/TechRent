using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechRent.Data;

namespace TechRent.Controllers
{
    [Authorize]
    public class ReportesController : Controller
    {
        private readonly AppDbContext _context;

        public ReportesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {

            // Total de equipos
            ViewBag.TotalEquipos = await _context.Equipos.CountAsync();

            // Total de clientes
            ViewBag.TotalClientes = await _context.Clientes.CountAsync();

            // Suma total de montos de reservas
            ViewBag.SumaTotalReservas = await _context.Reservas.SumAsync(r => r.MontoTotal);

            // Promedio de ventas por reserva
            ViewBag.PromedioVentas = await _context.Reservas.AverageAsync(r => r.MontoTotal);

            // Total de reservas
            ViewBag.TotalReservas = await _context.Reservas.CountAsync();

            // Total de marcas
            ViewBag.TotalMarcas = await _context.Marcas.CountAsync();

            // Total de categorías
            ViewBag.TotalCategorias = await _context.Categorias.CountAsync();

            // 5 equipos más alquilados (por cantidad de reservas)
            var equiposMasAlquilados = await _context.DetalleReservas
                .GroupBy(d => d.EquipoId)
                .Select(g => new
                {
                    EquipoId = g.Key,
                    TotalAlquileres = g.Count()
                })
                .OrderByDescending(x => x.TotalAlquileres)
                .Take(5)
                .ToListAsync();

            var equipoIds = equiposMasAlquilados.Select(e => e.EquipoId).ToList();
            var equipos = await _context.Equipos
                .Where(e => equipoIds.Contains(e.Id))
                .ToDictionaryAsync(e => e.Id, e => e.Nombre);

            ViewBag.EquiposMasAlquilados = equiposMasAlquilados.Select(e => new
            {
                Nombre = equipos.ContainsKey(e.EquipoId) ? equipos[e.EquipoId] : "Desconocido",
                e.TotalAlquileres
            }).ToList();

            return View();
        }
    }
}
