namespace MiukaFotoRazor.Models;

public class CategoryTreeNode
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<CategoryTreeNode> Children { get; set; } = [];
}
