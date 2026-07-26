#nullable disable

using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using TechRent.Services;

namespace TechRent.Areas.Identity.Pages.Account.Manage
{
    public class GenerateRecoveryCodesModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<GenerateRecoveryCodesModel> _logger;
        private readonly IEmailNotificationService _emailNotification;

        public GenerateRecoveryCodesModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILogger<GenerateRecoveryCodesModel> logger, IEmailNotificationService emailNotification)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailNotification = emailNotification;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound($"No se pudo cargar el usuario.");

            var isEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
            if (!isEnabled) return RedirectToPage("./TwoFactorAuthentication");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound($"No se pudo cargar el usuario.");

            var codes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            if (codes == null) return BadRequest("No se pudieron generar codigos de recuperacion.");

            _logger.LogInformation("Usuario genero nuevos codigos de recuperacion.");
            await _emailNotification.SendMfaActivatedNotificationAsync(user.Email!);

            return RedirectToPage("./ShowRecoveryCodes", new { recoveryCodes = codes.ToArray() });
        }
    }
}
