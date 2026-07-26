using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechRent.Data;

namespace TechRent.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminMfaController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppDbContext _context;

        public AdminMfaController(UserManager<IdentityUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, string? search = null, bool? mfaEnabled = null)
        {
            int pageSize = 20;

            var query = _context.Users
                .AsNoTracking()
                .OrderBy(u => u.Email)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(u => u.Email != null && u.Email.Contains(search));

            if (mfaEnabled.HasValue)
                query = query.Where(u => u.TwoFactorEnabled == mfaEnabled.Value);

            var totalRegistros = await query.CountAsync();

            var usuarios = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.TotalRegistros = totalRegistros;
            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRegistros / pageSize);
            ViewBag.Search = search;
            ViewBag.MfaEnabled = mfaEnabled;

            return View(usuarios);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivarMfa(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            await _userManager.SetTwoFactorEnabledAsync(user, true);
            TempData["Exito"] = $"MFA activado para {user.Email}.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DesactivarMfa(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            await _userManager.SetTwoFactorEnabledAsync(user, false);
            TempData["Exito"] = $"MFA desactivado para {user.Email}.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetClaveAuthenticator(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            await _userManager.SetTwoFactorEnabledAsync(user, false);
            await _userManager.ResetAuthenticatorKeyAsync(user);
            TempData["Exito"] = $"Clave del autenticador restablecida para {user.Email}. Se desactivó 2FA, debe configurarlo de nuevo.";
            return RedirectToAction(nameof(Index));
        }
    }
}
