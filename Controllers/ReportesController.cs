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
            ViewBag.TotalEquipos = await _context.Equipos.CountAsync();
            ViewBag.TotalClientes = await _context.Clientes.CountAsync();
            ViewBag.SumaTotalReservas = await _context.Reservas.SumAsync(r => r.MontoTotal);
            ViewBag.PromedioVentas = await _context.Reservas.AverageAsync(r => r.MontoTotal);
            ViewBag.TotalReservas = await _context.Reservas.CountAsync();
            ViewBag.TotalMarcas = await _context.Marcas.CountAsync();
            ViewBag.TotalCategorias = await _context.Categorias.CountAsync();

            var equiposMasAlquilados = await _context.DetalleReservas
                .GroupBy(d => d.EquipoId)
                .Select(g => new { EquipoId = g.Key, TotalAlquileres = g.Count() })
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

        public async Task<IActionResult> VentasPorFecha(
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null,
            string? agrupacion = null)
        {
            var query = _context.Reservas
                .Include(r => r.EstadoReserva)
                .AsNoTracking()
                .AsQueryable();

            if (fechaDesde.HasValue)
                query = query.Where(r => r.FechaCreacion >= fechaDesde.Value);
            if (fechaHasta.HasValue)
                query = query.Where(r => r.FechaCreacion <= fechaHasta.Value.AddDays(1));

            var agrupar = agrupacion ?? "dia";

            var todos = await query.Select(r => new { r.FechaCreacion, r.MontoTotal }).ToListAsync();

            IEnumerable<dynamic> resultados;
            if (agrupar == "semana")
            {
                resultados = todos
                    .GroupBy(r => System.Globalization.ISOWeek.GetWeekOfYear(r.FechaCreacion))
                    .Select(g => new { Fecha = g.First().FechaCreacion, Cantidad = g.Count(), Monto = g.Sum(r => r.MontoTotal) })
                    .OrderByDescending(x => x.Fecha)
                    .ToList();
            }
            else if (agrupar == "mes")
            {
                resultados = todos
                    .GroupBy(r => new { r.FechaCreacion.Year, r.FechaCreacion.Month })
                    .Select(g => new { Fecha = new DateTime(g.Key.Year, g.Key.Month, 1), Cantidad = g.Count(), Monto = g.Sum(r => r.MontoTotal) })
                    .OrderByDescending(x => x.Fecha)
                    .ToList();
            }
            else
            {
                resultados = todos
                    .GroupBy(r => r.FechaCreacion.Date)
                    .Select(g => new { Fecha = g.Key, Cantidad = g.Count(), Monto = g.Sum(r => r.MontoTotal) })
                    .OrderByDescending(x => x.Fecha)
                    .ToList();
            }

            ViewBag.FechaDesde = fechaDesde?.ToString("yyyy-MM-dd");
            ViewBag.FechaHasta = fechaHasta?.ToString("yyyy-MM-dd");
            ViewBag.Agrupacion = agrupar;
            ViewBag.TotalReservas = resultados.Sum(r => r.Cantidad);
            ViewBag.TotalMonto = resultados.Sum(r => r.Monto);
            ViewData["Resultados"] = resultados;

            return View();
        }

        public async Task<IActionResult> IngresosPorMes(
            int? anio = null,
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null)
        {
            var query = _context.Reservas
                .AsNoTracking()
                .Where(r => r.FechaCreacion.Year >= (anio ?? DateTime.UtcNow.Year - 2));

            if (fechaDesde.HasValue)
                query = query.Where(r => r.FechaCreacion >= fechaDesde.Value);
            if (fechaHasta.HasValue)
                query = query.Where(r => r.FechaCreacion <= fechaHasta.Value.AddDays(1));

            var resultados = await query
                .GroupBy(r => new { r.FechaCreacion.Year, r.FechaCreacion.Month })
                .Select(g => new
                {
                    Anio = g.Key.Year,
                    Mes = g.Key.Month,
                    TotalReservas = g.Count(),
                    MontoTotal = g.Sum(r => r.MontoTotal),
                    MontoPromedio = g.Average(r => r.MontoTotal)
                })
                .OrderByDescending(x => x.Anio).ThenByDescending(x => x.Mes)
                .ToListAsync();

            ViewBag.Anio = anio;
            ViewBag.FechaDesde = fechaDesde?.ToString("yyyy-MM-dd");
            ViewBag.FechaHasta = fechaHasta?.ToString("yyyy-MM-dd");
            ViewData["Resultados"] = resultados;

            return View();
        }

        public async Task<IActionResult> ClientesTopCompras(
            int top = 10,
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null)
        {
            var query = _context.Reservas
                .AsNoTracking()
                .AsQueryable();

            if (fechaDesde.HasValue)
                query = query.Where(r => r.FechaCreacion >= fechaDesde.Value);
            if (fechaHasta.HasValue)
                query = query.Where(r => r.FechaCreacion <= fechaHasta.Value.AddDays(1));

            var rankings = await query
                .GroupBy(r => r.ClienteId)
                .Select(g => new { ClienteId = g.Key, TotalReservas = g.Count(), MontoTotal = g.Sum(r => r.MontoTotal) })
                .OrderByDescending(x => x.TotalReservas)
                .Take(top)
                .ToListAsync();

            var clienteIds = rankings.Select(r => r.ClienteId).ToList();
            var clientes = await _context.Clientes
                .Where(c => clienteIds.Contains(c.Id))
                .ToDictionaryAsync(c => c.Id, c => c.NombreCompleto);

            var resultadoFinal = rankings.Select(r => new
            {
                Posicion = rankings.IndexOf(r) + 1,
                Nombre = clientes.ContainsKey(r.ClienteId) ? clientes[r.ClienteId] : "Desconocido",
                r.TotalReservas,
                r.MontoTotal,
                PromedioPorReserva = r.TotalReservas > 0 ? r.MontoTotal / r.TotalReservas : 0
            }).ToList();

            ViewBag.Top = top;
            ViewBag.FechaDesde = fechaDesde?.ToString("yyyy-MM-dd");
            ViewBag.FechaHasta = fechaHasta?.ToString("yyyy-MM-dd");
            ViewData["Resultados"] = resultadoFinal;

            return View();
        }

        public async Task<IActionResult> InventarioBajo(
            int umbral = 5,
            int? categoriaId = null,
            int? marcaId = null)
        {
            var query = _context.Equipos
                .Include(e => e.Categoria)
                .Include(e => e.Marca)
                .AsNoTracking()
                .Where(e => e.Stock <= umbral);

            if (categoriaId.HasValue)
                query = query.Where(e => e.CategoriaId == categoriaId.Value);
            if (marcaId.HasValue)
                query = query.Where(e => e.MarcaId == marcaId.Value);

            var equipos = await query
                .OrderBy(e => e.Stock)
                .ToListAsync();

            ViewBag.Umbral = umbral;
            ViewBag.CategoriaId = categoriaId;
            ViewBag.MarcaId = marcaId;
            ViewBag.Categorias = await _context.Categorias.ToListAsync();
            ViewBag.Marcas = await _context.Marcas.ToListAsync();

            return View(equipos);
        }

        public async Task<IActionResult> IntentosAccesoFallidos(
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null,
            int pageNumber = 1)
        {
            int pageSize = 25;

            var query = _context.AuditLogs
                .AsNoTracking()
                .Where(a => a.Accion == "Acceso denegado" || a.Accion == "Cuenta bloqueada");

            if (fechaDesde.HasValue)
                query = query.Where(a => a.Fecha >= fechaDesde.Value);
            if (fechaHasta.HasValue)
                query = query.Where(a => a.Fecha <= fechaHasta.Value.AddDays(1));

            var totalRegistros = await query.CountAsync();

            var registros = await query
                .OrderByDescending(a => a.Fecha)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var resumenPorAccion = await query
                .GroupBy(a => a.Accion)
                .Select(g => new { Accion = g.Key, Cantidad = g.Count() })
                .ToListAsync();

            ViewBag.TotalRegistros = totalRegistros;
            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRegistros / pageSize);
            ViewBag.FechaDesde = fechaDesde?.ToString("yyyy-MM-dd");
            ViewBag.FechaHasta = fechaHasta?.ToString("yyyy-MM-dd");
            ViewBag.ResumenPorAccion = resumenPorAccion;
            ViewData["Registros"] = registros;

            return View();
        }

        public async Task<IActionResult> TiempoPromedioIA(
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null,
            int pageNumber = 1)
        {
            int pageSize = 25;

            var query = _context.AuditLogs
                .AsNoTracking()
                .Where(a => a.Entidad == "IA");

            if (fechaDesde.HasValue)
                query = query.Where(a => a.Fecha >= fechaDesde.Value);
            if (fechaHasta.HasValue)
                query = query.Where(a => a.Fecha <= fechaHasta.Value.AddDays(1));

            var totalRegistros = await query.CountAsync();

            var registros = await query
                .OrderByDescending(a => a.Fecha)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var tiempos = registros
                .Where(r => r.Detalles != null && r.Detalles.Contains("Tiempo:"))
                .Select(r =>
                {
                    var idx = r.Detalles!.IndexOf("Tiempo: ") + 8;
                    var fin = r.Detalles!.IndexOf("ms", idx);
                    if (fin > idx && long.TryParse(r.Detalles[idx..fin], out long ms))
                        return (long?)ms;
                    return null;
                })
                .Where(t => t.HasValue)
                .Select(t => t!.Value)
                .ToList();

            ViewBag.TotalRegistros = totalRegistros;
            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRegistros / pageSize);
            ViewBag.FechaDesde = fechaDesde?.ToString("yyyy-MM-dd");
            ViewBag.FechaHasta = fechaHasta?.ToString("yyyy-MM-dd");
            ViewBag.PromedioTiempo = tiempos.Any() ? tiempos.Average() : 0;
            ViewBag.MinTiempo = tiempos.Any() ? tiempos.Min() : 0;
            ViewBag.MaxTiempo = tiempos.Any() ? tiempos.Max() : 0;
            ViewBag.TotalConsultas = tiempos.Count;
            ViewData["Registros"] = registros;

            return View();
        }
    }
}
