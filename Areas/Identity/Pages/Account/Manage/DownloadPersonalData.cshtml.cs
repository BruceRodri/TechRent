#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace TechRent.Areas.Identity.Pages.Account.Manage
{
    public class DownloadPersonalDataModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<DownloadPersonalDataModel> _logger;

        public DownloadPersonalDataModel(UserManager<IdentityUser> userManager, ILogger<DownloadPersonalDataModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public IActionResult OnPost()
        {
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound($"No se pudo cargar el usuario.");

            _logger.LogInformation("Usuario solicitó descarga de datos personales.");

            var personalData = new Dictionary<string, string>
            {
                ["userId"] = user.Id,
                ["email"] = user.Email,
                ["userName"] = user.UserName,
                ["phoneNumber"] = user.PhoneNumber,
                ["emailConfirmed"] = user.EmailConfirmed.ToString(),
                ["phoneNumberConfirmed"] = user.PhoneNumberConfirmed.ToString(),
                ["twoFactorEnabled"] = user.TwoFactorEnabled.ToString(),
                ["lockoutEnd"] = user.LockoutEnd?.ToString() ?? "null",
                ["lockoutEnabled"] = user.LockoutEnabled.ToString(),
                ["accessFailedCount"] = user.AccessFailedCount.ToString()
            };

            var logins = await _userManager.GetLoginsAsync(user);
            foreach (var login in logins)
            {
                personalData[$"login_{login.LoginProvider}"] = login.ProviderKey;
            }

            Response.Headers.Append("Content-Disposition", "attachment; filename=TechRent-datos-personales.json");
            var json = JsonSerializer.Serialize(personalData, new JsonSerializerOptions { WriteIndented = true });
            return File(Encoding.UTF8.GetBytes(json), "application/json", "TechRent-datos-personales.json");
        }
    }
}
