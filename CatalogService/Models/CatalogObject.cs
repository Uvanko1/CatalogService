namespace CatalogService.Models;

public class CatalogObject
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; } = null;
    public Category Category { get; set; } = null!;
    public int CategoryId { get; set; }
    public string? ImageKey { get; set; } = null;
}
