namespace CatalogService.Models;
using System.ComponentModel.DataAnnotations;

public class CatalogSubmission
{
    public int Id { get; set; }
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    [MaxLength(5000)]
    public string? Description { get; set; } = null;
    public int? CategoryId { get; set; } = null;
    [MaxLength(100)]
    public string? ProposedCategoryName { get; set; } = null;
    public string? ImageKey { get; set; } = null;
    public int SubmitterId { get; set; }
    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "pending";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? ResultObjectId { get; set; } = null;
    public string? RejectionReason { get; set; } = null;
}
