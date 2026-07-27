using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechRent.Data;
using TechRent.Models;
using TechRent.Services;

namespace TechRent.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ClientesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IAuditService _audit;

        public ClientesController(AppDbContext context, IAuditService audit)
        {
            _context = context;
            _audit = audit;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, string searchString = "")
        {

            int pageSize = 20;
            var query = _context.Clientes.AsNoTracking().Where(c => c.Activo);

            if (!string.IsNullOrEmpty(searchString))
            {
                var searchLower = searchString.ToLower();
                query = query.Where(c => c.NombreCompleto.ToLower().Contains(searchLower) ||
                                          c.Email.ToLower().Contains(searchLower) ||
                                          c.Telefono.ToLower().Contains(searchLower));
            }

            var totalRegistros = await query.CountAsync();
            var clientesConEmail = await query.CountAsync(c => c.Email != null);
            var clientesConTelefono = await query.CountAsync(c => c.Telefono != null);

            var clientes = await query
                .OrderBy(c => c.NombreCompleto)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.TotalRegistros = totalRegistros;
            ViewBag.ClientesConEmail = clientesConEmail;
            ViewBag.ClientesConTelefono = clientesConTelefono;
            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRegistros / pageSize);
            ViewBag.SearchString = searchString;

            return View(clientes);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var cliente = await _context.Clientes.FirstOrDefaultAsync(m => m.Id == id);
            if (cliente == null) return NotFound();
            return View(cliente);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cliente cliente)
        {
            ModelState.Remove("FechaCreacion");

            if (ModelState.IsValid)
            {
                cliente.FechaCreacion = DateTime.UtcNow;
                cliente.CreadoPor = User.Identity?.Name;
                _context.Add(cliente);
                await _context.SaveChangesAsync();
                await _audit.RegistrarAsync("Creacion de cliente", "Cliente", cliente.Id.ToString(), null, AuditService.SerializeObject(cliente), user: User, httpContext: HttpContext);
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();
            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Cliente cliente)
        {
            if (id != cliente.Id) return NotFound();

            ModelState.Remove("FechaCreacion");

            if (ModelState.IsValid)
            {
                try
                {
                    var dbCliente = await _context.Clientes.FindAsync(id);
                    if (dbCliente == null) return NotFound();

                    var antes = AuditService.SerializeObject(new { dbCliente.NombreCompleto, dbCliente.Email, dbCliente.Telefono, dbCliente.Direccion });

                    dbCliente.NombreCompleto = cliente.NombreCompleto;
                    dbCliente.Email = cliente.Email;
                    dbCliente.Telefono = cliente.Telefono;
                    dbCliente.Direccion = cliente.Direccion;
                    dbCliente.DocumentoIdentidad = cliente.DocumentoIdentidad;
                    dbCliente.Activo = cliente.Activo;
                    dbCliente.FechaActualizacion = DateTime.UtcNow;
                    dbCliente.ActualizadoPor = User.Identity?.Name;

                    await _context.SaveChangesAsync();

                    var despues = AuditService.SerializeObject(new { cliente.NombreCompleto, cliente.Email, cliente.Telefono, cliente.Direccion });
                    await _audit.RegistrarAsync("Modificacion de cliente", "Cliente", id.ToString(), antes, despues, user: User, httpContext: HttpContext);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var cliente = await _context.Clientes.FirstOrDefaultAsync(m => m.Id == id);
            if (cliente == null) return NotFound();
            return View(cliente);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente != null)
            {
                cliente.Activo = false;
                cliente.FechaEliminacion = DateTime.UtcNow;
                cliente.FechaActualizacion = DateTime.UtcNow;
                cliente.EliminadoPor = User.Identity?.Name;
                await _context.SaveChangesAsync();
                await _audit.RegistrarAsync("Eliminacion logica de cliente", "Cliente", id.ToString(), "Activo=true", "Activo=false", user: User, httpContext: HttpContext);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClienteExists(int id)
        {
            return _context.Clientes.Any(e => e.Id == id);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetCount()
        {
            var count = await _context.Clientes.CountAsync();
            return Ok(count);
        }
    }
}
