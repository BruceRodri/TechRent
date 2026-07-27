using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechRent.Data;
using TechRent.Models;
using TechRent.Services;

namespace TechRent.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class MarcasController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IAuditService _audit;

        public MarcasController(AppDbContext context, IAuditService audit)
        {
            _context = context;
            _audit = audit;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, string searchString = "")
        {

            int pageSize = 20;
            var query = _context.Marcas.AsNoTracking().Where(m => m.Activo);

            if (!string.IsNullOrEmpty(searchString))
            {
                var searchLower = searchString.ToLower();
                query = query.Where(m => m.Nombre.ToLower().Contains(searchLower) ||
                                          (m.PaisOrigen != null && m.PaisOrigen.ToLower().Contains(searchLower)));
            }

            var totalRegistros = await query.CountAsync();
            var items = await query
                .OrderBy(m => m.Nombre)
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
            var item = await _context.Marcas.FirstOrDefaultAsync(m => m.Id == id);
            if (item == null) return NotFound();
            return View(item);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Marca marca)
        {
            ModelState.Remove("FechaCreacion");

            if (ModelState.IsValid)
            {
                marca.FechaCreacion = DateTime.UtcNow;
                marca.CreadoPor = User.Identity?.Name;
                _context.Add(marca);
                await _context.SaveChangesAsync();
                await _audit.RegistrarAsync("Creacion de marca", "Marca", marca.Id.ToString(), null, AuditService.SerializeObject(marca), user: User, httpContext: HttpContext);
                return RedirectToAction(nameof(Index));
            }
            return View(marca);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var item = await _context.Marcas.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Marca marca)
        {
            if (id != marca.Id) return NotFound();

            ModelState.Remove("FechaCreacion");

            if (ModelState.IsValid)
            {
                try
                {
                    var dbMarca = await _context.Marcas.FindAsync(id);
                    if (dbMarca == null) return NotFound();

                    var antes = AuditService.SerializeObject(new { dbMarca.Nombre, dbMarca.PaisOrigen });

                    dbMarca.Nombre = marca.Nombre;
                    dbMarca.PaisOrigen = marca.PaisOrigen;
                    dbMarca.Activo = marca.Activo;
                    dbMarca.FechaActualizacion = DateTime.UtcNow;
                    dbMarca.ActualizadoPor = User.Identity?.Name;

                    await _context.SaveChangesAsync();

                    var despues = AuditService.SerializeObject(new { marca.Nombre, marca.PaisOrigen });
                    await _audit.RegistrarAsync("Modificacion de marca", "Marca", id.ToString(), antes, despues, user: User, httpContext: HttpContext);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Marcas.Any(e => e.Id == marca.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(marca);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var item = await _context.Marcas.FirstOrDefaultAsync(m => m.Id == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.Marcas.IgnoreQueryFilters().FirstOrDefaultAsync(m => m.Id == id);
            if (item != null)
            {
                item.Activo = false;
                item.FechaEliminacion = DateTime.UtcNow;
                item.FechaActualizacion = DateTime.UtcNow;
                item.EliminadoPor = User.Identity?.Name;
                await _context.SaveChangesAsync();
                await _audit.RegistrarAsync("Eliminacion logica de marca", "Marca", id.ToString(), "Activo=true", "Activo=false", user: User, httpContext: HttpContext);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        public async Task<IActionResult> GetCount()
        {
            var count = await _context.Marcas.CountAsync();
            return Ok(count);
        }
    }
}
