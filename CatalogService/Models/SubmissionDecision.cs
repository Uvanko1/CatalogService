namespace CatalogService.Models;
using Enums;

public class SubmissionDecision
{
    public int Id { get; set; }
    public Guid OperationId { get; set; }
    public int SubmissionId { get; set; }
    public SubmissionDecisionAction Action { get; set; }
    public int? ResolvedCategoryId { get; set; }
    public string? ResolvedCategoryName { get; set; }
    public int? ResultObjectId { get; set; }
    public int? ResultCategoryId { get; set; }
    public DateTime CreatedAt { get; set; }
}
