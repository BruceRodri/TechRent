using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechRent.Data;
using TechRent.Models;

namespace TechRent.Controllers
{
    [Authorize]
    public class MarcasController : Controller
    {
        private readonly AppDbContext _context;

        public MarcasController(AppDbContext context)
        {
            _context = context;
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
                _context.Add(marca);
                await _context.SaveChangesAsync();
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
