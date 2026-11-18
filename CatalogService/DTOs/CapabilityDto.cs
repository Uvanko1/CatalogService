namespace CatalogService.Models;

public class CapabilityDto
{
    public bool SupportsStatus { get; set; } = false;
    public List<string> AllowedStatuses { get; set; } = new List<string>();
}
