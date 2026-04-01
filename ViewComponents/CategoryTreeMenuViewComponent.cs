using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiukaFotoRazor.Data;
using MiukaFotoRazor.Models;
using Microsoft.Extensions.Caching.Memory;

namespace MiukaFotoRazor.ViewComponents;

public class CategoryTreeMenuViewComponent(MiukaFotoDbContext db, IMemoryCache cache) : ViewComponent
{
    private readonly MiukaFotoDbContext _db = db;
    private readonly IMemoryCache _cache = cache;

    public async Task<IViewComponentResult> InvokeAsync()
    {
        // 🔹 Cache key
        const string cacheKey = "CategoryTree_AlphaDoc";

        if (!_cache.TryGetValue(cacheKey, out List<CategoryTreeNode>? cachedTree))
        {
            // ✅ Build tree when cache miss
            var items = await (
                from i in _db.ArtCategoryItems.AsNoTracking().Where(x => x.IS_ACTIVE == true)
                orderby i.ORDER_NUM
                from l in _db.SysLanguages.AsNoTracking()
                    .Where(x => x.KEY == i.NAME_CODE).DefaultIfEmpty()
                select new ArtCategoryItemDTO
                {
                    Id = i.ID,
                    ParentId = i.PARENT_ID,
                    OrderNum = i.ORDER_NUM,
                    NameCode = i.NAME_CODE,
                    Name = l != null ? l.EN : i.NAME_CODE
                }
            ).ToListAsync();

            // ✅ Dùng List<long> thay vì HashSet<long>
            var subtreeIds = GetDescendantIds(items, rootId: 1);
            var filtered = items.Where(x => subtreeIds.Contains(x.Id)).ToList();
            var rootItem = filtered.FirstOrDefault(x => x.Id == 1);
            if (rootItem == null)
                return View(new List<CategoryTreeNode>());

            var rootNode = BuildNodeRecursive(filtered, rootItem);
            cachedTree = new List<CategoryTreeNode> { rootNode };

            // 🧠 Cache for 1 hour
            _cache.Set(cacheKey, cachedTree, TimeSpan.FromHours(1));
        }

        return View(cachedTree);
    }

    // 🐾 Recursive builder
    private CategoryTreeNode BuildNodeRecursive(List<ArtCategoryItemDTO> items, ArtCategoryItemDTO current)
    {
        return new CategoryTreeNode
        {
            Id = current.Id,
            Name = current.Name,
            Children = items
                .Where(x => x.ParentId == current.Id)
                .OrderBy(x => x.OrderNum)
                .Select(child => BuildNodeRecursive(items, child))
                .ToList()
        };
    }

    // ✅ Trả về List<long> thay vì HashSet
    private List<long> GetDescendantIds(List<ArtCategoryItemDTO> flatItems, long rootId)
    {
        var descendants = new List<long> { rootId };
        var queue = new Queue<long>();
        queue.Enqueue(rootId);

        while (queue.Count > 0)
        {
            var parent = queue.Dequeue();
            var children = flatItems.Where(x => x.ParentId == parent).Select(x => x.Id);

            foreach (var childId in children)
            {
                if (!descendants.Contains(childId))
                {
                    descendants.Add(childId);
                    queue.Enqueue(childId);
                }
            }
        }

        return descendants;
    }
}
