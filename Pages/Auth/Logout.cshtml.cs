using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MiukaFotoRazor.Pages.Auth;

// Validate anti-forgery for all POST handlers on this page
[ValidateAntiForgeryToken]
public class LogoutModel : PageModel
{
    public IActionResult OnPost()
    {
        HttpContext.Session.Clear();
        return RedirectToPage("/Index");
    }
}
