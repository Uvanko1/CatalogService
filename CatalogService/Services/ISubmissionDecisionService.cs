using CatalogService.DTOs;
using CatalogService.Models;

namespace CatalogService.Services;

public interface ISubmissionDecisionService
{
    Task<SubmissionDecision> ApplyDecisionAsync(int submissionId, SubmissionDecisionDto submissionDecision);
}
