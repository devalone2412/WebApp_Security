using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp_UnderTheHood;

namespace MyApp.Namespace
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Credential Credential { get; set; } = new Credential();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // Verify the credential
            if (Credential.UserName == "admin" && Credential.Password == "password")
            {

                // Creating the security context
                var claims = new List<Claim> {
                    new (ClaimTypes.Name, "admin"),
                    new (ClaimTypes.Email, "admin@mywebsite.com"),
                    new ("Department", "HR"),
                    new ("Admin", "true"),
                    new ("Manager", "true"),
                    new ("EmploymentDate", "2023-05-01")
                };

                var identity = new ClaimsIdentity(claims, AuthScheme.CookieScheme);
                var claimsPrincipal = new ClaimsPrincipal(identity);

                var authProperties = new AuthenticationProperties {
                    IsPersistent = Credential.RememberMe
                };

                await HttpContext.SignInAsync(AuthScheme.CookieScheme, claimsPrincipal, authProperties);

                return RedirectToPage("/Index");
            }

            return Page();
        }
    }

    public class Credential
    {
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set;}
    }
}