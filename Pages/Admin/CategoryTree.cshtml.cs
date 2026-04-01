using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiukaFotoRazor.Data;
using MiukaFotoRazor.Models;

namespace MiukaFotoRazor.Pages.Admin;

public class CategoryTreeModel(MiukaFotoDbContext db) : BasePageModel
{
    private readonly MiukaFotoDbContext _db = db;

    public List<CategoryTreeNode> Tree { get; set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        var isRoot = HttpContext.Session.GetString("IsRoot");
        if (string.IsNullOrEmpty(isRoot) || isRoot != "True")
        {
            return RedirectToPage("/Auth/Login");
        }

        var items = await _db.ArtCategoryItems
            .AsNoTracking()
            .Where(x => x.IS_ACTIVE == true)
            .ToListAsync();

        Tree = BuildTree(items, parentId: null);
        return Page();
    }

    private List<CategoryTreeNode> BuildTree(List<ART_CATEGORY_ITEM> flatItems, long? parentId)
    {
        return flatItems
            .Where(x => x.PARENT_ID == parentId)
            .OrderBy(x => x.ORDER_NUM)
            .Select(x => new CategoryTreeNode
            {
                Id = x.ID,
                Name = x.NAME_CODE,
                Children = BuildTree(flatItems, x.ID)
            })
            .ToList();
    }
}
