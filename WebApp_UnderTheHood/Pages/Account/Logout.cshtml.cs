using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp_UnderTheHood;

namespace MyApp.Namespace
{
    public class LogoutModel : PageModel
    {
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync() {
            await HttpContext.SignOutAsync(AuthScheme.CookieScheme);
            return RedirectToPage("/Index");
        }
    }
}
