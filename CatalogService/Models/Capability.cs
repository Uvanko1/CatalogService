namespace CatalogService.Models;

public class Capability
{
    public bool SupportsStatus { get; set; } = false;
    public List<string> AllowedStatuses { get; set; } = new List<string>();
}
