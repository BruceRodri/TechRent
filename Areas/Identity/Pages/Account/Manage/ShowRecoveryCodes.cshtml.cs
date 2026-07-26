#nullable disable

using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TechRent.Areas.Identity.Pages.Account.Manage
{
    public class ShowRecoveryCodesModel : PageModel
    {
        public string[] RecoveryCodes { get; set; }

        public IActionResult OnGet(string[] recoveryCodes)
        {
            if (recoveryCodes == null || recoveryCodes.Length == 0)
                return RedirectToPage("./TwoFactorAuthentication");

            RecoveryCodes = recoveryCodes;
            return Page();
        }
    }
}
