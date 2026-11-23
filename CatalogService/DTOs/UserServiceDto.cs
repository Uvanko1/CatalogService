namespace CatalogService.DTOs;

public class UserServiceDto
{
    public bool IsValid { get; set; }
    public string? UserId { get; set; }
    public List<string> Privileges { get; set; } = new List<string>();
}
