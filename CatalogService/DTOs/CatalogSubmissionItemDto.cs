namespace CatalogService.DTOs;

public class CatalogSubmissionItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? CategoryId { get; set; }
    public string? ProposedCategoryName { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? ImageUrl { get; set; }
    public int? ResultObjectId { get; set; }
    public string? RejectionReason { get; set; }
}