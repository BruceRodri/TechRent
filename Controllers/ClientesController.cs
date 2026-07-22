using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechRent.Data;
using TechRent.Models;

namespace TechRent.Controllers
{
    [Authorize]
    public class ClientesController : Controller
    {
        private readonly AppDbContext _context;

        public ClientesController(AppDbContext context)
        {
            _context = context;
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
                _context.Add(cliente);
                await _context.SaveChangesAsync();
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

                    dbCliente.NombreCompleto = cliente.NombreCompleto;
                    dbCliente.Email = cliente.Email;
                    dbCliente.Telefono = cliente.Telefono;
                    dbCliente.Direccion = cliente.Direccion;
                    dbCliente.DocumentoIdentidad = cliente.DocumentoIdentidad;
                    dbCliente.Activo = cliente.Activo;
                    dbCliente.FechaActualizacion = DateTime.UtcNow;

                    await _context.SaveChangesAsync();
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
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClienteExists(int id)
        {
            return _context.Clientes.Any(e => e.Id == id);
        }

        [HttpGet]
        public async Task<IActionResult> GetCount()
        {
            var count = await _context.Clientes.CountAsync();
            return Ok(count);
        }
    }
}
