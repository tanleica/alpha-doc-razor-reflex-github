using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MiukaFotoRazor.Data;
using MiukaFotoRazor.Models;

namespace MiukaFotoRazor.Pages.Topic;

public class ArticlesByTopicModel(MiukaFotoDbContext db) : PageModel
{
    public List<ART_ARTICLE> Articles { get; set; } = [];
    public string? TopicName { get; set; }

    public async Task<IActionResult> OnGetAsync(long topicId)
    {
        var topic = await db.ArtCategoryItems.FirstOrDefaultAsync(x => x.ID == topicId);
        if (topic == null) return NotFound();

        TopicName = topic.NAME_CODE;

        Articles = await db.ArtArticles
            .Where(x => x.TOPIC_ID == topicId)
            .OrderByDescending(x => x.CREATED_DATE)
            .ToListAsync();

        return Page();
    }
}

