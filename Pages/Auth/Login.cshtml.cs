using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiukaFotoRazor.Data;

public class LoginModel(MiukaFotoDbContext db) : BasePageModel
{
    private readonly MiukaFotoDbContext _db = db;
    [BindProperty] public required string Username { get; set; }
    [BindProperty] public required string Password { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _db.SysUsers.FirstOrDefaultAsync(x => x.USERNAME == Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(Password, user.PASSWORDHASH))
        {
            ErrorMessage = "Invalid username or password.";
            return Page();
        }

        HttpContext.Session.SetString("UserId", user.ID);
        HttpContext.Session.SetString("IsRoot", user.IS_ROOT == true ? "True" : "False");
        //return RedirectToPage("/Admin/CategoryTree");
        return RedirectToPage("/Index");
    }
}
