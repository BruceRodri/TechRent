using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechRent.Data;
using TechRent.Models;

namespace TechRent.Controllers
{
    [Authorize]
    public class EquiposController : Controller
    {
        private readonly AppDbContext _context;

        public EquiposController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, string searchString = "")
        {

            int pageSize = 20;
            var query = _context.Equipos
                .Include(e => e.Categoria)
                .Include(e => e.Marca)
                .AsNoTracking()
                .Where(e => e.Activo);

            if (!string.IsNullOrEmpty(searchString))
            {
                var searchLower = searchString.ToLower();
                query = query.Where(e => e.Nombre.ToLower().Contains(searchLower) ||
                                          (e.Descripcion != null && e.Descripcion.ToLower().Contains(searchLower)));
            }

            var totalRegistros = await query.CountAsync();

            var aggQuery = _context.Equipos.AsNoTracking().Where(e => e.Activo);
            if (!string.IsNullOrEmpty(searchString))
            {
                var searchLower = searchString.ToLower();
                aggQuery = aggQuery.Where(e => e.Nombre.ToLower().Contains(searchLower) ||
                                                (e.Descripcion != null && e.Descripcion.ToLower().Contains(searchLower)));
            }
            var totalStock = await aggQuery.SumAsync(e => e.Stock);
            var precioPromedio = await aggQuery.AverageAsync(e => (decimal?)e.PrecioPorDia) ?? 0;

            var equipos = await query
                .OrderBy(e => e.Nombre)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.TotalRegistros = totalRegistros;
            ViewBag.TotalStock = totalStock;
            ViewBag.PrecioPromedio = precioPromedio;
            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRegistros / pageSize);
            ViewBag.SearchString = searchString;

            return View(equipos);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var equipo = await _context.Equipos
                .Include(e => e.Categoria)
                .Include(e => e.Marca)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (equipo == null) return NotFound();
            return View(equipo);
        }

        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre");
            ViewData["MarcaId"] = new SelectList(_context.Marcas, "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(bool? _ = null)
        {
            var nombre = Request.Form["Nombre"].ToString();
            var descripcion = Request.Form["Descripcion"].ToString();
            var precioStr = Request.Form["PrecioPorDia"].ToString();
            var stockStr = Request.Form["Stock"].ToString();
            var especificaciones = Request.Form["Especificaciones"].ToString();
            var activo = Request.Form.ContainsKey("Activo");
            var categoriaIdStr = Request.Form["CategoriaId"].ToString();
            var marcaIdStr = Request.Form["MarcaId"].ToString();

            bool precioOk = decimal.TryParse(precioStr, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var precio);
            bool stockOk = int.TryParse(stockStr, out var stock);
            bool catOk = int.TryParse(categoriaIdStr, out var categoriaId);
            bool marOk = int.TryParse(marcaIdStr, out var marcaId);

            var equipo = new Equipo
            {
                Nombre = nombre,
                Descripcion = string.IsNullOrEmpty(descripcion) ? null : descripcion,
                PrecioPorDia = precioOk ? precio : 0,
                Stock = stockOk ? stock : 0,
                Especificaciones = string.IsNullOrEmpty(especificaciones) ? null : especificaciones,
                Activo = true,
                CategoriaId = catOk ? categoriaId : 0,
                MarcaId = marOk ? marcaId : 0,
                FechaCreacion = DateTime.UtcNow
            };

            // Manually validate the fields we care about
            ModelState.Clear();

            if (string.IsNullOrWhiteSpace(nombre))
                ModelState.AddModelError("Nombre", "El nombre del equipo es obligatorio");
            if (string.IsNullOrWhiteSpace(descripcion))
                ModelState.AddModelError("Descripcion", "La descripción es obligatoria");
            if (!precioOk || precio <= 0)
                ModelState.AddModelError("PrecioPorDia", "Ingrese un precio válido mayor a 0");
            if (!stockOk || stock < 0)
                ModelState.AddModelError("Stock", "Ingrese un stock válido");
            if (!catOk || categoriaId <= 0)
                ModelState.AddModelError("CategoriaId", "Seleccione una categoría");
            if (!marOk || marcaId <= 0)
                ModelState.AddModelError("MarcaId", "Seleccione una marca");

            if (ModelState.IsValid)
            {
                _context.Add(equipo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre", equipo.CategoriaId);
            ViewData["MarcaId"] = new SelectList(_context.Marcas, "Id", "Nombre", equipo.MarcaId);
            return View(equipo);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var equipo = await _context.Equipos.FindAsync(id);
            if (equipo == null) return NotFound();
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre", equipo.CategoriaId);
            ViewData["MarcaId"] = new SelectList(_context.Marcas, "Id", "Nombre", equipo.MarcaId);
            return View(equipo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Equipo equipo)
        {
            if (id != equipo.Id) return NotFound();

            if (equipo.CategoriaId == 0 && int.TryParse(Request.Form["CategoriaId"], out var catId))
                equipo.CategoriaId = catId;
            if (equipo.MarcaId == 0 && int.TryParse(Request.Form["MarcaId"], out var marId))
                equipo.MarcaId = marId;

            ModelState.Remove("CategoriaId");
            ModelState.Remove("MarcaId");
            ModelState.Remove("FechaCreacion");

            if (equipo.CategoriaId == 0)
                ModelState.AddModelError("CategoriaId", "La categoría es requerida");
            if (equipo.MarcaId == 0)
                ModelState.AddModelError("MarcaId", "La marca es requerida");

            if (ModelState.IsValid)
            {
                try
                {
                    var dbEquipo = await _context.Equipos.FindAsync(id);
                    if (dbEquipo == null) return NotFound();

                    dbEquipo.Nombre = equipo.Nombre;
                    dbEquipo.Descripcion = equipo.Descripcion;
                    dbEquipo.PrecioPorDia = equipo.PrecioPorDia;
                    dbEquipo.Stock = equipo.Stock;
                    dbEquipo.Especificaciones = equipo.Especificaciones;
                    dbEquipo.Activo = equipo.Activo;
                    dbEquipo.CategoriaId = equipo.CategoriaId;
                    dbEquipo.MarcaId = equipo.MarcaId;
                    dbEquipo.FechaActualizacion = DateTime.UtcNow;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EquipoExists(equipo.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre", equipo.CategoriaId);
            ViewData["MarcaId"] = new SelectList(_context.Marcas, "Id", "Nombre", equipo.MarcaId);
            return View(equipo);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var equipo = await _context.Equipos
                .Include(e => e.Categoria)
                .Include(e => e.Marca)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (equipo == null) return NotFound();
            return View(equipo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var equipo = await _context.Equipos.IgnoreQueryFilters().FirstOrDefaultAsync(e => e.Id == id);
            if (equipo != null)
            {
                equipo.Activo = false;
                equipo.FechaEliminacion = DateTime.UtcNow;
                equipo.FechaActualizacion = DateTime.UtcNow;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EquipoExists(int id) => _context.Equipos.Any(e => e.Id == id);

        [HttpGet]
        public async Task<IActionResult> GetCount()
        {
            var count = await _context.Equipos.CountAsync();
            return Ok(count);
        }
    }
}
