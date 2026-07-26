#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace TechRent.Areas.Identity.Pages.Account.Manage
{
    public class EmailModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public EmailModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        public string Email { get; set; }
        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Nuevo correo")]
            public string NewEmail { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            var email = await _userManager.GetEmailAsync(user);
            Email = email;
            Input = new InputModel { NewEmail = email };
            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound($"No se pudo cargar el usuario.");
            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostChangeEmailAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound($"No se pudo cargar el usuario.");

            if (!ModelState.IsValid) { await LoadAsync(user); return Page(); }

            var email = await _userManager.GetEmailAsync(user);
            if (Input.NewEmail != email)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page("/Account/ConfirmEmailChange", pageHandler: null, values: new { area = "Identity", userId = userId, email = Input.NewEmail, code = code }, protocol: Request.Scheme);
                await _emailSender.SendEmailAsync(Input.NewEmail, "Confirma tu nuevo correo", $"Haz clic en el enlace para confirmar: <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Confirmar</a>.");
                StatusMessage = "Se ha enviado un enlace de confirmacion a tu nuevo correo.";
                return RedirectToPage();
            }

            StatusMessage = "Tu correo no ha cambiado.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound($"No se pudo cargar el usuario.");

            if (!ModelState.IsValid) { await LoadAsync(user); return Page(); }

            var userId = await _userManager.GetUserIdAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page("/Account/ConfirmEmail", pageHandler: null, values: new { area = "Identity", userId = userId, code = code }, protocol: Request.Scheme);
            await _emailSender.SendEmailAsync(email, "Confirma tu correo", $"Haz clic en el enlace para confirmar: <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Confirmar</a>.");
            StatusMessage = "Correo de verificacion enviado. Revisa tu bandeja de entrada.";
            return RedirectToPage();
        }
    }
}
