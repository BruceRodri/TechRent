using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechRent.Data;
using TechRent.Models;

namespace TechRent.Controllers
{
    public class MarcasController : Controller
    {
        private readonly AppDbContext _context;

        public MarcasController(AppDbContext context)
        {
            _context = context;
        }

        private IActionResult? VerificarSesion()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UsuarioNombre")))
                return RedirectToAction("Login", "Auth");
            return null;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, string searchString = "")
        {
            var sesion = VerificarSesion();
            if (sesion != null) return sesion;

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
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRegistros / pageSize);
            ViewBag.SearchString = searchString;

            return View(items);
        }

        public async Task<IActionResult> Details(int? id)
        {
            var sesion = VerificarSesion();
            if (sesion != null) return sesion;
            if (id == null) return NotFound();
            var item = await _context.Marcas.FirstOrDefaultAsync(m => m.Id == id);
            if (item == null) return NotFound();
            return View(item);
        }

        public IActionResult Create()
        {
            var sesion = VerificarSesion();
            if (sesion != null) return sesion;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Marca marca)
        {
            var sesion = VerificarSesion();
            if (sesion != null) return sesion;

            ModelState.Remove("FechaCreacion");

            if (ModelState.IsValid)
            {
                marca.FechaCreacion = DateTime.UtcNow;
                _context.Add(marca);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(marca);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            var sesion = VerificarSesion();
            if (sesion != null) return sesion;
            if (id == null) return NotFound();
            var item = await _context.Marcas.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Marca marca)
        {
            var sesion = VerificarSesion();
            if (sesion != null) return sesion;
            if (id != marca.Id) return NotFound();

            ModelState.Remove("FechaCreacion");

            if (ModelState.IsValid)
            {
                try
                {
                    var dbMarca = await _context.Marcas.FindAsync(id);
                    if (dbMarca == null) return NotFound();

                    dbMarca.Nombre = marca.Nombre;
                    dbMarca.PaisOrigen = marca.PaisOrigen;
                    dbMarca.Activo = marca.Activo;
                    dbMarca.FechaActualizacion = DateTime.UtcNow;

                    await _context.SaveChangesAsync();
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
            var sesion = VerificarSesion();
            if (sesion != null) return sesion;
            if (id == null) return NotFound();
            var item = await _context.Marcas.FirstOrDefaultAsync(m => m.Id == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sesion = VerificarSesion();
            if (sesion != null) return sesion;
            var item = await _context.Marcas.IgnoreQueryFilters().FirstOrDefaultAsync(m => m.Id == id);
            if (item != null)
            {
                item.Activo = false;
                item.FechaEliminacion = DateTime.UtcNow;
                item.FechaActualizacion = DateTime.UtcNow;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> GetCount()
        {
            var count = await _context.Marcas.CountAsync();
            return Ok(count);
        }
    }
}
