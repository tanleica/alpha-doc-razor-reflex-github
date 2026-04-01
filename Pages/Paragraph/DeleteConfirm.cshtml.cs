using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MiukaFotoRazor.Data;

namespace MiukaFotoRazor.Pages.Paragraph;

public class DeleteConfirmModel : PageModel
{
    private readonly MiukaFotoDbContext _db;

    public DeleteConfirmModel(MiukaFotoDbContext db)
    {
        _db = db;
    }

    [BindProperty(SupportsGet = true)]
    public long Id { get; set; }

    [BindProperty(SupportsGet = true)]
    public long ArticleId { get; set; }

    public string? PreviewBody { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var p = await _db.ArtParagraphs
            .FirstOrDefaultAsync(x => x.ID == Id);

        if (p == null)
            return NotFound();

        PreviewBody = p.BODY;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var p = await _db.ArtParagraphs.FindAsync(Id);

        if (p != null)
        {
            _db.ArtParagraphs.Remove(p);
            await _db.SaveChangesAsync();
        }

        return RedirectToPage("/Article", new { id = ArticleId });
    }
}