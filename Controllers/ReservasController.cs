using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechRent.Data;
using TechRent.Models;
using TechRent.Services;

namespace TechRent.Controllers
{
    [Authorize]
    public class ReservasController : Controller
    {
        private readonly AppDbContext _context;
        private readonly InventarioService _inventario;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailNotificationService _emailNotification;

        public ReservasController(AppDbContext context, InventarioService inventario, UserManager<IdentityUser> userManager, IEmailNotificationService emailNotification)
        {
            _context = context;
            _inventario = inventario;
            _userManager = userManager;
            _emailNotification = emailNotification;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, string searchString = "")
        {

            int pageSize = 20;
            var query = _context.Reservas
                .Include(r => r.Cliente)
                .Include(r => r.EstadoReserva)
                .Include(r => r.DetalleReservas)
                    .ThenInclude(d => d.Equipo)
                .AsNoTracking()
                .Where(r => r.Activo);

            if (!string.IsNullOrEmpty(searchString))
            {
                var searchLower = searchString.ToLower();
                query = query.Where(r => r.Cliente.NombreCompleto.ToLower().Contains(searchLower) ||
                                          (r.Observaciones != null && r.Observaciones.ToLower().Contains(searchLower)));
            }

            var totalRegistros = await query.CountAsync();
            var items = await query
                .OrderByDescending(r => r.FechaCreacion)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.TotalRegistros = totalRegistros;
            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRegistros / pageSize);
            ViewBag.SearchString = searchString;

            return View(items);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var item = await _context.Reservas
                .Include(r => r.Cliente)
                .Include(r => r.EstadoReserva)
                .Include(r => r.DetalleReservas)
                    .ThenInclude(d => d.Equipo)
                .Include(r => r.Pagos)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null) return NotFound();
            return View(item);
        }

        public IActionResult Create()
        {
            ViewData["EstadoReservaId"] = new SelectList(_context.EstadosReserva.Where(e => e.Activo), "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reserva reserva)
        {
            ModelState.Remove("FechaCreacion");
            ModelState.Remove("MontoTotal");
            ModelState.Remove("Cliente");
            ModelState.Remove("EstadoReserva");

            var equipoIds = Request.Form["equipoIds"];
            var cantidades = Request.Form["cantidades"];
            var tieneEquipos = equipoIds.Count > 0;

            if (ModelState.IsValid && tieneEquipos)
            {
                var cantidadDias = (reserva.FechaFin.Date - reserva.FechaInicio.Date).Days;
                if (cantidadDias <= 0)
                {
                    ModelState.AddModelError("FechaFin", "La fecha fin debe ser posterior a la fecha inicio");
                    ViewData["EstadoReservaId"] = new SelectList(_context.EstadosReserva.Where(e => e.Activo), "Id", "Nombre", reserva.EstadoReservaId);
                    return View(reserva);
                }

                await using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    reserva.FechaCreacion = DateTime.UtcNow;
                    reserva.FechaInicio = DateTime.SpecifyKind(reserva.FechaInicio, DateTimeKind.Utc);
                    reserva.FechaFin = DateTime.SpecifyKind(reserva.FechaFin, DateTimeKind.Utc);
                    reserva.MontoTotal = 0;

                    _context.Add(reserva);
                    await _context.SaveChangesAsync();

                    var userId = _userManager.GetUserId(User);

                    for (int i = 0; i < equipoIds.Count; i++)
                    {
                        if (!int.TryParse(equipoIds[i], out var equipoId) || !int.TryParse(cantidades[i], out var cantidad)) continue;
                        var equipo = await _context.Equipos.FindAsync(equipoId);
                        if (equipo == null || equipo.Stock < cantidad) continue;

                        equipo.Stock -= cantidad;
                        _inventario.RegistrarMovimiento(equipo, "Venta aprobada", -cantidad,
                            referencia: $"Reserva #{reserva.Id}",
                            observacion: $"Se descontaron {cantidad} unidades por reserva",
                            usuarioId: userId);

                        if (equipo.Stock <= 5)
                        {
                            await EnviarAlertaInventarioCriticoAsync(equipo.Nombre, equipo.Stock);
                        }

                        var subtotal = cantidad * equipo.PrecioPorDia * cantidadDias;
                        var detalle = new DetalleReserva
                        {
                            ReservaId = reserva.Id,
                            EquipoId = equipoId,
                            Cantidad = cantidad,
                            CantidadDias = cantidadDias,
                            PrecioUnitarioPorDia = equipo.PrecioPorDia,
                            Subtotal = subtotal
                        };
                        _context.Add(detalle);
                        reserva.MontoTotal += subtotal;
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            if (!tieneEquipos)
                ModelState.AddModelError("", "Debe seleccionar al menos un equipo");

            ViewData["EstadoReservaId"] = new SelectList(_context.EstadosReserva.Where(e => e.Activo), "Id", "Nombre", reserva.EstadoReservaId);
            return View(reserva);
        }

        [HttpGet]
        public async Task<IActionResult> GetClientes(string term)
        {
            if (string.IsNullOrWhiteSpace(term)) return Ok(new List<object>());
            var searchLower = term.ToLower();
            var clientes = await _context.Clientes
                .AsNoTracking()
                .Where(c => c.Activo && c.NombreCompleto.ToLower().Contains(searchLower))
                .OrderBy(c => c.NombreCompleto)
                .Take(20)
                .Select(c => new { c.Id, Nombre = c.NombreCompleto, c.Email })
                .ToListAsync();
            return Ok(clientes);
        }

        [HttpGet]
        public async Task<IActionResult> GetEquiposDisponibles(string term, string incluirIds)
        {
            var idsIncluir = new List<int>();
            if (!string.IsNullOrEmpty(incluirIds))
            {
                foreach (var idStr in incluirIds.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    if (int.TryParse(idStr, out var id)) idsIncluir.Add(id);
                }
            }

            IQueryable<Equipo> query;

            if (idsIncluir.Any())
            {
                query = _context.Equipos
                    .AsNoTracking()
                    .Include(e => e.Categoria)
                    .Include(e => e.Marca)
                    .Where(e => e.Activo && (e.Stock > 0 || idsIncluir.Contains(e.Id)));
            }
            else
            {
                query = _context.Equipos
                    .AsNoTracking()
                    .Include(e => e.Categoria)
                    .Include(e => e.Marca)
                    .Where(e => e.Activo && e.Stock > 0);
            }

            if (!string.IsNullOrWhiteSpace(term))
            {
                var searchLower = term.ToLower();
                query = query.Where(e => e.Nombre.ToLower().Contains(searchLower));
            }

            var equipos = await query
                .OrderBy(e => e.Nombre)
                .Take(200)
                .Select(e => new { e.Id, e.Nombre, e.PrecioPorDia, e.Stock, Categoria = e.Categoria.Nombre, Marca = e.Marca.Nombre })
                .ToListAsync();

            // Asegurar que los equipos seleccionados estén siempre incluidos
            if (idsIncluir.Any())
            {
                var idsPresentes = equipos.Select(e => e.Id).ToHashSet();
                var idsFaltantes = idsIncluir.Where(id => !idsPresentes.Contains(id)).ToList();
                if (idsFaltantes.Any())
                {
                    var faltantes = await _context.Equipos
                        .AsNoTracking()
                        .Include(e => e.Categoria)
                        .Include(e => e.Marca)
                        .Where(e => idsFaltantes.Contains(e.Id))
                        .Select(e => new { e.Id, e.Nombre, e.PrecioPorDia, e.Stock, Categoria = e.Categoria.Nombre, Marca = e.Marca.Nombre })
                        .ToListAsync();
                    equipos = faltantes.Concat(equipos).ToList();
                }
            }

            return Ok(equipos);
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var item = await _context.Reservas
                .Include(r => r.Cliente)
                .Include(r => r.DetalleReservas)
                .ThenInclude(d => d.Equipo)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null) return NotFound();
            ViewData["EstadoReservaId"] = new SelectList(_context.EstadosReserva.Where(e => e.Activo), "Id", "Nombre", item.EstadoReservaId);
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id, Reserva reserva)
        {
            if (id != reserva.Id) return NotFound();

            ModelState.Remove("FechaCreacion");
            ModelState.Remove("Cliente");
            ModelState.Remove("EstadoReserva");

            var equipoIds = Request.Form["equipoIds"];
            var cantidades = Request.Form["cantidades"];
            var tieneEquipos = equipoIds.Count > 0;

            if (ModelState.IsValid && tieneEquipos)
            {
                var cantidadDias = (reserva.FechaFin.Date - reserva.FechaInicio.Date).Days;
                if (cantidadDias <= 0)
                {
                    ModelState.AddModelError("FechaFin", "La fecha fin debe ser posterior a la fecha inicio");
                    ViewData["EstadoReservaId"] = new SelectList(_context.EstadosReserva.Where(e => e.Activo), "Id", "Nombre", reserva.EstadoReservaId);
                    return View(reserva);
                }

                try
                {
                    await using var transaction = await _context.Database.BeginTransactionAsync();

                    var dbReserva = await _context.Reservas
                        .Include(r => r.DetalleReservas)
                        .FirstOrDefaultAsync(r => r.Id == id);
                    if (dbReserva == null) { await transaction.RollbackAsync(); return NotFound(); }

                    var userId = _userManager.GetUserId(User);

                    foreach (var detalle in dbReserva.DetalleReservas)
                    {
                        var equipo = await _context.Equipos.FindAsync(detalle.EquipoId);
                        if (equipo != null)
                        {
                            equipo.Stock += detalle.Cantidad;
                            _inventario.RegistrarMovimiento(equipo, "Devolución aprobada", detalle.Cantidad,
                                referencia: $"Edición reserva #{dbReserva.Id}",
                                observacion: $"Se reintegraron {detalle.Cantidad} unidades al editar reserva",
                                usuarioId: userId);
                        }
                    }
                    _context.DetalleReservas.RemoveRange(dbReserva.DetalleReservas);

                    dbReserva.FechaInicio = DateTime.SpecifyKind(reserva.FechaInicio, DateTimeKind.Utc);
                    dbReserva.FechaFin = DateTime.SpecifyKind(reserva.FechaFin, DateTimeKind.Utc);
                    dbReserva.Observaciones = reserva.Observaciones;
                    dbReserva.ClienteId = reserva.ClienteId;
                    dbReserva.EstadoReservaId = reserva.EstadoReservaId;
                    dbReserva.FechaActualizacion = DateTime.UtcNow;
                    dbReserva.MontoTotal = 0;

                    for (int i = 0; i < equipoIds.Count; i++)
                    {
                        if (!int.TryParse(equipoIds[i], out var equipoId) || !int.TryParse(cantidades[i], out var cantidad)) continue;
                        var equipo = await _context.Equipos.FindAsync(equipoId);
                        if (equipo == null || equipo.Stock < cantidad) continue;

                        equipo.Stock -= cantidad;
                        _inventario.RegistrarMovimiento(equipo, "Venta aprobada", -cantidad,
                            referencia: $"Reserva #{dbReserva.Id}",
                            observacion: $"Se descontaron {cantidad} unidades al editar reserva",
                            usuarioId: userId);

                        var subtotal = cantidad * equipo.PrecioPorDia * cantidadDias;
                        _context.Add(new DetalleReserva
                        {
                            ReservaId = dbReserva.Id,
                            EquipoId = equipoId,
                            Cantidad = cantidad,
                            CantidadDias = cantidadDias,
                            PrecioUnitarioPorDia = equipo.PrecioPorDia,
                            Subtotal = subtotal,
                            FechaCreacion = DateTime.UtcNow
                        });
                        dbReserva.MontoTotal += subtotal;
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Reservas.Any(e => e.Id == reserva.Id)) return NotFound();
                    else throw;
                }
            }

            if (!tieneEquipos)
                ModelState.AddModelError("", "Debe seleccionar al menos un equipo");

            ViewData["EstadoReservaId"] = new SelectList(_context.EstadosReserva.Where(e => e.Activo), "Id", "Nombre", reserva.EstadoReservaId);
            return View(reserva);
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var item = await _context.Reservas
                .Include(r => r.Cliente)
                .Include(r => r.EstadoReserva)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.Reservas.IgnoreQueryFilters().FirstOrDefaultAsync(r => r.Id == id);
            if (item != null)
            {
                item.Activo = false;
                item.FechaEliminacion = DateTime.UtcNow;
                item.FechaActualizacion = DateTime.UtcNow;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservaExists(int id) => _context.Reservas.Any(e => e.Id == id);

        private async Task EnviarAlertaInventarioCriticoAsync(string equipoNombre, int stockActual)
        {
            try
            {
                var adminEmails = await _userManager.GetUsersInRoleAsync("Administrador");
                foreach (var admin in adminEmails)
                {
                    if (!string.IsNullOrWhiteSpace(admin.Email))
                    {
                        await _emailNotification.SendCriticalInventoryAlertAsync(
                            admin.Email, equipoNombre, stockActual);
                    }
                }
            }
            catch { }
        }
    }
}
