using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MiukaFotoRazor.Data;
using MiukaFotoRazor.Models;


namespace MiukaFotoRazor.Pages;

public class IndexModel : BasePageModel
{
    private readonly MiukaFotoDbContext _db;
    private readonly IMemoryCache _cache;

    public IndexModel(MiukaFotoDbContext db, IMemoryCache cache)
    {
        _db = db;
        _cache = cache;
    }

    [BindProperty(SupportsGet = true)]
    public string? Keyword { get; set; }

    public bool IsRoot { get; private set; }

    public List<ART_ARTICLE> Articles { get; set; } = [];

    public async Task OnGetAsync()
    {
        // 1️⃣ Lấy danh sách topic con (cache 1 tiếng)
        const string cacheKey = "TopicIdSet_AlphaDoc";
        if (!_cache.TryGetValue(cacheKey, out List<long>? categoryIds))
        {
            categoryIds = await GetDescendantCategoryIdsAsync(1);
            if (!categoryIds.Contains(1))
                categoryIds.Add(1);
            _cache.Set(cacheKey, categoryIds, TimeSpan.FromHours(1));
        }

        // 2️⃣ Build câu SQL thủ công an toàn
        var inClause = string.Join(",", categoryIds);

        var baseSql = $@"
            SELECT *
            FROM art.ART_ARTICLE
            WHERE TOPIC_ID IN ({inClause})
        ";

        var list = await _db.ArtArticles
            .FromSqlRaw(baseSql)
            .Include(a => a.Paragraphs)
            .AsNoTracking()
            .ToListAsync();

        // 4️⃣ Lọc keyword phía client
        if (!string.IsNullOrWhiteSpace(Keyword))
        {
            var kw = Keyword.ToUpperInvariant();
            list = list
                .Where(a =>
                    (a.BODY != null && a.BODY.ToUpperInvariant().Contains(kw)) ||
                    (a.Paragraphs != null &&
                    a.Paragraphs.Any(p => p.BODY != null && p.BODY.ToUpperInvariant().Contains(kw))))
                .ToList();
        }

        // 5️⃣ Sắp xếp & giới hạn kết quả (pin mới nhất luôn trước)
        Articles = list
            .OrderByDescending(a => a.PINNED)       // nhóm pinned lên đầu
            .ThenByDescending(a => a.CREATED_DATE)  // trong mỗi nhóm, bài mới nhất trước
            .Take(20)
            .ToList();


        // 6️⃣ Kiểm tra quyền root
        var isRootStr = HttpContext.Session.GetString("IsRoot");
        IsRoot = string.Equals(isRootStr, "True", StringComparison.OrdinalIgnoreCase);
    }

    // 🔁 Truy xuất tất cả category con dưới root
    private async Task<List<long>> GetDescendantCategoryIdsAsync(long rootId)
    {
        var all = await _db.ArtCategoryItems
            .AsNoTracking()
            .Where(c => c.IS_ACTIVE == true)
            .Select(c => new { c.ID, c.PARENT_ID })
            .ToListAsync();

        var result = new List<long> { rootId };
        var queue = new Queue<long>();
        queue.Enqueue(rootId);

        while (queue.Count > 0)
        {
            var parent = queue.Dequeue();
            foreach (var child in all.Where(c => c.PARENT_ID == parent))
            {
                if (!result.Contains(child.ID))
                {
                    result.Add(child.ID);
                    queue.Enqueue(child.ID);
                }
            }
        }

        return result;
    }

    // 🔤 Decode placeholder trong nội dung bài
    public static string DecodeBody(string raw)
    {
        if (string.IsNullOrEmpty(raw))
            return string.Empty;

        return raw.Replace("||1", "<");
    }
}
