#nullable disable

using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace TechRent.Areas.Identity.Pages.Account.Manage
{
    public class Disable2faModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<Disable2faModel> _logger;

        public Disable2faModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILogger<Disable2faModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound($"No se pudo cargar el usuario.");

            if (!await _userManager.GetTwoFactorEnabledAsync(user))
                return RedirectToPage("./TwoFactorAuthentication");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound($"No se pudo cargar el usuario.");

            var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!disable2faResult.Succeeded) return BadRequest("Error al desactivar la autenticacion de dos factores.");

            _logger.LogInformation("Usuario desactivo la autenticacion de dos factores.");
            StatusMessage = "La autenticacion de dos factores fue desactivada.";
            return RedirectToPage();
        }
    }
}
