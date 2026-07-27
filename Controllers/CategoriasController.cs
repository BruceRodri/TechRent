using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechRent.Data;
using TechRent.Models;
using TechRent.Services;

namespace TechRent.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class CategoriasController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IAuditService _audit;

        public CategoriasController(AppDbContext context, IAuditService audit)
        {
            _context = context;
            _audit = audit;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, string searchString = "")
        {

            int pageSize = 20;
            var query = _context.Categorias.AsNoTracking().Where(c => c.Activo);

            if (!string.IsNullOrEmpty(searchString))
            {
                var searchLower = searchString.ToLower();
                query = query.Where(c => c.Nombre.ToLower().Contains(searchLower));
            }

            var totalRegistros = await query.CountAsync();
            var items = await query
                .OrderBy(c => c.Nombre)
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
            var item = await _context.Categorias.FirstOrDefaultAsync(m => m.Id == id);
            if (item == null) return NotFound();
            return View(item);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Categoria categoria)
        {
            ModelState.Remove("FechaCreacion");

            if (ModelState.IsValid)
            {
                categoria.FechaCreacion = DateTime.UtcNow;
                categoria.CreadoPor = User.Identity?.Name;
                _context.Add(categoria);
                await _context.SaveChangesAsync();
                await _audit.RegistrarAsync("Creacion de categoria", "Categoria", categoria.Id.ToString(), null, AuditService.SerializeObject(categoria), user: User, httpContext: HttpContext);
                return RedirectToAction(nameof(Index));
            }
            return View(categoria);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var item = await _context.Categorias.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Categoria categoria)
        {
            if (id != categoria.Id) return NotFound();

            ModelState.Remove("FechaCreacion");

            if (ModelState.IsValid)
            {
                try
                {
                    var dbCategoria = await _context.Categorias.FindAsync(id);
                    if (dbCategoria == null) return NotFound();

                    var antes = AuditService.SerializeObject(new { dbCategoria.Nombre, dbCategoria.Descripcion });

                    dbCategoria.Nombre = categoria.Nombre;
                    dbCategoria.Descripcion = categoria.Descripcion;
                    dbCategoria.Activo = categoria.Activo;
                    dbCategoria.FechaActualizacion = DateTime.UtcNow;
                    dbCategoria.ActualizadoPor = User.Identity?.Name;

                    await _context.SaveChangesAsync();

                    var despues = AuditService.SerializeObject(new { categoria.Nombre, categoria.Descripcion });
                    await _audit.RegistrarAsync("Modificacion de categoria", "Categoria", id.ToString(), antes, despues, user: User, httpContext: HttpContext);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Categorias.Any(e => e.Id == categoria.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(categoria);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var item = await _context.Categorias.FirstOrDefaultAsync(m => m.Id == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.Categorias.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Id == id);
            if (item != null)
            {
                item.Activo = false;
                item.FechaEliminacion = DateTime.UtcNow;
                item.FechaActualizacion = DateTime.UtcNow;
                item.EliminadoPor = User.Identity?.Name;
                await _context.SaveChangesAsync();
                await _audit.RegistrarAsync("Eliminacion logica de categoria", "Categoria", id.ToString(), "Activo=true", "Activo=false", user: User, httpContext: HttpContext);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        public async Task<IActionResult> GetCount()
        {
            var count = await _context.Categorias.CountAsync();
            return Ok(count);
        }
    }
}
