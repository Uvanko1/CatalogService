using CatalogService.DTOs;
using CatalogService.Models;

namespace CatalogService.Services;

public interface ICatalogSubmissionService
{
    Task<CatalogSubmission> CreateSubmissionAsync(
        CatalogSubmissionDto submissionDto,
        Guid submitterId
    );
    Task<List<CatalogSubmissionItemDto>> GetMySubmissionsAsync(Guid submitterId);
}
