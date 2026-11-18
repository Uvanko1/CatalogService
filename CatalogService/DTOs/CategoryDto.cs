namespace CatalogService.DTOs;
using CatalogService.Models;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public CapabilityDto Capabilities { get; set; } = new CapabilityDto();
}
