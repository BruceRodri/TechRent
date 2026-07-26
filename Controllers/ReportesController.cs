using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechRent.Data;

namespace TechRent.Controllers
{
    [Authorize(Roles = "Administrador")]
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

        public async Task<IActionResult> Transacciones(
            string? proveedor = null,
            string? estado = null,
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null,
            int pageNumber = 1)
        {
            int pageSize = 20;

            var query = _context.TransaccionesPago
                .Include(t => t.OrdenAlquiler)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrEmpty(proveedor))
                query = query.Where(t => t.Proveedor == proveedor);

            if (!string.IsNullOrEmpty(estado))
                query = query.Where(t => t.Estado == estado);

            if (fechaDesde.HasValue)
                query = query.Where(t => t.FechaCreacion >= fechaDesde.Value);

            if (fechaHasta.HasValue)
                query = query.Where(t => t.FechaCreacion <= fechaHasta.Value.AddDays(1));

            var totalRegistros = await query.CountAsync();

            var transacciones = await query
                .OrderByDescending(t => t.FechaCreacion)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Resumen por pasarela
            var resumenPorPasarela = await _context.TransaccionesPago
                .GroupBy(t => t.Proveedor)
                .Select(g => new
                {
                    Proveedor = g.Key,
                    TotalTransacciones = g.Count(),
                    MontoTotal = g.Sum(t => t.MontoEnCentavos) / 100m,
                    Aprobados = g.Count(t => t.Estado == "Pagado"),
                    Pendientes = g.Count(t => t.Estado == "Pendiente"),
                    Cancelados = g.Count(t => t.Estado == "Cancelado"),
                    Fallidos = g.Count(t => t.Estado == "Fallido"),
                    Expirados = g.Count(t => t.Estado == "Expirado"),
                    Reembolsados = g.Count(t => t.Estado == "Reembolsado"),
                    MontoAprobado = g.Where(t => t.Estado == "Pagado").Sum(t => t.MontoEnCentavos) / 100m,
                    MontoCancelado = g.Where(t => t.Estado == "Cancelado").Sum(t => t.MontoEnCentavos) / 100m,
                    MontoFallido = g.Where(t => t.Estado == "Fallido").Sum(t => t.MontoEnCentavos) / 100m,
                    MontoReembolsado = g.Where(t => t.Estado == "Reembolsado").Sum(t => t.MontoEnCentavos) / 100m
                })
                .ToListAsync();

            // Resumen por estado
            var resumenPorEstado = await _context.TransaccionesPago
                .GroupBy(t => t.Estado)
                .Select(g => new
                {
                    Estado = g.Key,
                    Cantidad = g.Count(),
                    MontoTotal = g.Sum(t => t.MontoEnCentavos) / 100m
                })
                .ToListAsync();

            ViewBag.TotalRegistros = totalRegistros;
            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRegistros / pageSize);
            ViewBag.Proveedor = proveedor;
            ViewBag.Estado = estado;
            ViewBag.FechaDesde = fechaDesde?.ToString("yyyy-MM-dd");
            ViewBag.FechaHasta = fechaHasta?.ToString("yyyy-MM-dd");
            ViewBag.ResumenPorPasarela = resumenPorPasarela;
            ViewBag.ResumenPorEstado = resumenPorEstado;

            return View(transacciones);
        }
    }
}
