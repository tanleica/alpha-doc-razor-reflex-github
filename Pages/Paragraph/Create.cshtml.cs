using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MiukaFotoRazor.Data;
using MiukaFotoRazor.Models;

namespace MiukaFotoRazor.Pages.Paragraph;

public class CreateModel : PageModel
{
    private readonly MiukaFotoDbContext _db;

    public CreateModel(MiukaFotoDbContext db)
    {
        _db = db;
    }

    [BindProperty(SupportsGet = true)]
    public long ArticleId { get; set; }

    [BindProperty(SupportsGet = true)]
    public long? AfterId { get; set; }

    [BindProperty]
    public string Body { get; set; } = "";

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        short newQueue;

        if (AfterId.HasValue)
        {
            var after = await _db.ArtParagraphs
                .FirstAsync(x => x.ID == AfterId.Value);

            newQueue = (short)(after.QUEUE + 1);

            var affected = await _db.ArtParagraphs
                .Where(x => x.ARTICLE_ID == ArticleId && x.QUEUE > after.QUEUE)
                .ToListAsync();

            foreach (var p in affected)
            {
                p.QUEUE += 1;
            }
        }
        else
        {
            var maxQueue = await _db.ArtParagraphs
                .Where(x => x.ARTICLE_ID == ArticleId)
                .MaxAsync(x => (short?)x.QUEUE) ?? 0;

            newQueue = (short)(maxQueue + 10);
        }

        var paragraph = new ART_PARAGRAPH
        {
            ARTICLE_ID = ArticleId,
            BODY = Body,
            QUEUE = newQueue,
            CREATED_DATE = DateTime.UtcNow
        };

        _db.ArtParagraphs.Add(paragraph);
        await _db.SaveChangesAsync();

        return RedirectToPage("/Article", new { id = ArticleId });
    }
}