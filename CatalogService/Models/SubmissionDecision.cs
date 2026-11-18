namespace CatalogService.Models;

using System.ComponentModel.DataAnnotations;
public class SubmissionDecision
{
    public int Id { get; set; }
    public int SubmissionId { get; set; }
    [Required]
    [MaxLength(20)]
    public string Decision { get; set; } = string.Empty;
    public int ModeratorId { get; set; }
    public Guid OperationId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
