namespace CatalogService.Models;

using System.ComponentModel.DataAnnotations;

public class Category
{
    public int Id { get; set; }
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;
    public Capability Capabilities { get; set; } = new Capability();
    public ICollection<CatalogObject> CatalogObjects { get; set; } = new List<CatalogObject>();
}
