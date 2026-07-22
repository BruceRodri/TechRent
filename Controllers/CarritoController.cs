using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechRent.Data;
using TechRent.Models;

namespace TechRent.Controllers
{
    [Authorize]
    public class CarritoController : Controller
    {
        private readonly AppDbContext _context;

        public CarritoController(AppDbContext context)
        {
            _context = context;
        }

        private string GetEmail() => User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name ?? "";

        public async Task<IActionResult> Index()
        {
            var email = GetEmail();
            var items = await _context.CarritoItems
                .Include(c => c.Equipo)
                    .ThenInclude(e => e.Categoria)
                .Include(c => c.Equipo)
                    .ThenInclude(e => e.Marca)
                .Where(c => c.UsuarioEmail == email)
                .ToListAsync();

            decimal total = items.Sum(i => i.Cantidad * i.Equipo.PrecioPorDia * i.Dias);
            ViewBag.Total = total;

            return View(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Agregar(int equipoId, int cantidad = 1, int dias = 1)
        {
            var email = GetEmail();

            var equipo = await _context.Equipos.FindAsync(equipoId);
            if (equipo == null || !equipo.Activo || equipo.Stock < cantidad)
            {
                TempData["Error"] = "Equipo no disponible o stock insuficiente.";
                return RedirectToAction("Details", "Equipos", new { id = equipoId });
            }

            var existente = await _context.CarritoItems
                .FirstOrDefaultAsync(c => c.UsuarioEmail == email && c.EquipoId == equipoId);

            if (existente != null)
            {
                existente.Cantidad += cantidad;
                existente.Dias = dias;
                existente.FechaAgregado = DateTime.UtcNow;
            }
            else
            {
                _context.CarritoItems.Add(new CarritoItem
                {
                    UsuarioEmail = email,
                    EquipoId = equipoId,
                    Cantidad = cantidad,
                    Dias = dias
                });
            }

            await _context.SaveChangesAsync();
            TempData["Exito"] = $"{equipo.Nombre} agregado al carrito.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Actualizar(int id, int cantidad, int dias)
        {
            var email = GetEmail();
            var item = await _context.CarritoItems
                .Include(c => c.Equipo)
                .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioEmail == email);

            if (item == null) return NotFound();

            if (cantidad < 1) cantidad = 1;
            if (dias < 1) dias = 1;

            if (cantidad > item.Equipo.Stock)
            {
                TempData["Error"] = $"Stock insuficiente. Solo hay {item.Equipo.Stock} unidades.";
                return RedirectToAction(nameof(Index));
            }

            item.Cantidad = cantidad;
            item.Dias = dias;
            await _context.SaveChangesAsync();

            TempData["Exito"] = "Carrito actualizado.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var email = GetEmail();
            var item = await _context.CarritoItems
                .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioEmail == email);

            if (item != null)
            {
                _context.CarritoItems.Remove(item);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Equipo eliminado del carrito.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Vaciar()
        {
            var email = GetEmail();
            var items = await _context.CarritoItems
                .Where(c => c.UsuarioEmail == email)
                .ToListAsync();

            _context.CarritoItems.RemoveRange(items);
            await _context.SaveChangesAsync();

            TempData["Exito"] = "Carrito vaciado.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Contador()
        {
            var email = GetEmail();
            var count = await _context.CarritoItems
                .Where(c => c.UsuarioEmail == email)
                .SumAsync(c => c.Cantidad);
            return Json(count);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarReserva(string metodoPago = "PayPal")
        {
            var email = GetEmail();
            var items = await _context.CarritoItems
                .Include(c => c.Equipo)
                .Where(c => c.UsuarioEmail == email)
                .ToListAsync();

            if (!items.Any())
            {
                TempData["Error"] = "El carrito está vacío.";
                return RedirectToAction(nameof(Index));
            }

            decimal total = items.Sum(i => i.Cantidad * i.Equipo.PrecioPorDia * i.Dias);

            var orden = new OrdenAlquiler
            {
                UsuarioEmail = email,
                Total = total,
                Estado = "Pendiente",
                FechaCreacion = DateTime.UtcNow
            };

            foreach (var item in items)
            {
                orden.Detalles.Add(new DetalleOrdenAlquiler
                {
                    EquipoId = item.EquipoId,
                    NombreEquipo = item.Equipo.Nombre,
                    Cantidad = item.Cantidad,
                    Dias = item.Dias,
                    PrecioPorDia = item.Equipo.PrecioPorDia,
                    Subtotal = item.Cantidad * item.Equipo.PrecioPorDia * item.Dias
                });
            }

            _context.OrdenesAlquiler.Add(orden);
            _context.CarritoItems.RemoveRange(items);
            await _context.SaveChangesAsync();

            if (metodoPago == "PayPhone")
                return RedirectToAction("CreateLink", "Pago", new { ordenId = orden.Id });

            return RedirectToAction("PayPalButton", "Pago", new { ordenId = orden.Id });
        }
    }
}
