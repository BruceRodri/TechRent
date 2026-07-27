using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechRent.Data;
using TechRent.Services;

namespace TechRent.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminMfaController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppDbContext _context;
        private readonly IAuditService _audit;

        public AdminMfaController(UserManager<IdentityUser> userManager, AppDbContext context, IAuditService audit)
        {
            _userManager = userManager;
            _context = context;
            _audit = audit;
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
            await _audit.RegistrarAsync("Activacion de MFA por administrador", "IdentityUser", userId, "TwoFactorEnabled=false", "TwoFactorEnabled=true", $"Usuario: {user.Email}", User, HttpContext);
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
            await _audit.RegistrarAsync("Desactivacion de MFA por administrador", "IdentityUser", userId, "TwoFactorEnabled=true", "TwoFactorEnabled=false", $"Usuario: {user.Email}", User, HttpContext);
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
            await _audit.RegistrarAsync("Reset de autenticador por administrador", "IdentityUser", userId, "TwoFactorEnabled=true", "TwoFactorEnabled=false + AuthenticatorKey reset", $"Usuario: {user.Email}", User, HttpContext);
            TempData["Exito"] = $"Clave del autenticador restablecida para {user.Email}. Se desactivó 2FA, debe configurarlo de nuevo.";
            return RedirectToAction(nameof(Index));
        }
    }
}
