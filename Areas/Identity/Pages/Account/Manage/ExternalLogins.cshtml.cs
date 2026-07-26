#nullable disable

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TechRent.Areas.Identity.Pages.Account.Manage
{
    public class ExternalLoginsModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IUserStore<IdentityUser> _userStore;

        public ExternalLoginsModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IUserStore<IdentityUser> userStore)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userStore = userStore;
        }

        public IList<UserLoginInfo> CurrentLogins { get; set; }
        public IList<AuthenticationScheme> OtherLogins { get; set; }
        public bool ShowRemoveButton { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound($"No se pudo cargar el usuario.");

            CurrentLogins = await _userManager.GetLoginsAsync(user);
            OtherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
                .Where(auth => CurrentLogins.All(ul => auth.Name != ul.LoginProvider)).ToList();

            string passwordHash = null;
            if (_userStore is IUserPasswordStore<IdentityUser> userPasswordStore)
                passwordHash = await userPasswordStore.GetPasswordHashAsync(user, HttpContext.RequestAborted);

            ShowRemoveButton = passwordHash != null || CurrentLogins.Count > 1;
            return Page();
        }

        public async Task<IActionResult> OnPostRemoveLoginAsync(string loginProvider, string providerKey)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound($"No se pudo cargar el usuario.");

            var result = await _userManager.RemoveLoginAsync(user, loginProvider, providerKey);
            if (!result.Succeeded) { StatusMessage = "No se pudo eliminar el login externo."; return RedirectToPage(); }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "El login externo ha sido eliminado.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostLinkLoginAsync(string provider)
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            var redirectUrl = Url.Page("./ExternalLogins", pageHandler: "LinkLoginCallback");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(User));
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetLinkLoginCallbackAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound($"No se pudo cargar el usuario.");

            var userId = await _userManager.GetUserIdAsync(user);
            var info = await _signInManager.GetExternalLoginInfoAsync(userId);
            if (info == null) throw new InvalidOperationException("Error al cargar la informacion del login externo.");

            var result = await _userManager.AddLoginAsync(user, info);
            if (!result.Succeeded) { StatusMessage = "El login externo no fue agregado."; return RedirectToPage(); }

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            StatusMessage = "El login externo fue agregado.";
            return RedirectToPage();
        }
    }
}
