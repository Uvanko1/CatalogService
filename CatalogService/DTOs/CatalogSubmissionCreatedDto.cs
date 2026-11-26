namespace CatalogService.DTOs;

public class CatalogSubmissionCreatedDto
{
    public int Id { get; set; }
    public string Status { get; set; } = "pending";
    public DateTime CreatedAt { get; set; }
}
