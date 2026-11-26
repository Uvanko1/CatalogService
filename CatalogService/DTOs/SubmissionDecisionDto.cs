namespace CatalogService.DTOs;

using CatalogService.Models.Enums;

public class SubmissionDecisionDto
{
    public Guid OperationId { get; set; }
    public SubmissionDecisionAction Action { get; set; }
    public int? ResolvedCategoryId { get; set; }
    public string? ResolvedCategoryName { get; set; }
    public string? RejectionReason { get; set; }
}
