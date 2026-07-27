using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TechRent.Services;

namespace TechRent.Controllers
{
    public class AuthController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAuditService _audit;

        public AuthController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IAuditService audit)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _audit = audit;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string? returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Auth", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(User));
            return Challenge(properties, provider);
        }

        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
        {
            if (remoteError != null)
            {
                TempData["Error"] = $"Error de Google: {remoteError}";
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                TempData["Error"] = "Error al obtener informacion del proveedor externo";
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true, bypassTwoFactor: false);

            if (result.Succeeded)
            {
                var googleEmail = info.Principal.FindFirstValue(System.Security.Claims.ClaimTypes.Email);
                await _audit.RegistrarAsync("Inicio de sesion exitoso - Google", detalles: $"Provider: {info.LoginProvider}, Email: {googleEmail}", user: info.Principal, httpContext: HttpContext);
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                return RedirectToAction("Index", "Home");
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToPage("/Account/LoginWith2fa", new { area = "Identity", ReturnUrl = returnUrl, RememberMe = true });
            }

            var email = info.Principal.FindFirstValue(System.Security.Claims.ClaimTypes.Email);

            if (string.IsNullOrEmpty(email))
            {
                TempData["Error"] = "No se pudo obtener el email de la cuenta de Google";
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = false
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    TempData["Error"] = "Error al crear la cuenta: " + string.Join(", ", createResult.Errors.Select(e => e.Description));
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(
                    Microsoft.AspNetCore.WebUtilities.WebEncoders.Base64UrlEncode(System.Text.Encoding.UTF8.GetBytes(code))));
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { area = "Identity", userId = user.Id, code }, protocol: Request.Scheme);
            }

                await _userManager.AddLoginAsync(user, info);
                await _signInManager.SignInAsync(user, isPersistent: true);
                await _audit.RegistrarAsync("Registro de usuario via Google", "IdentityUser", user.Id, null, $"Email: {email}", null, info.Principal, HttpContext);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
