using CatalogService.DTOs;
using CatalogService.Models;

namespace CatalogService.Services;

public class CatalogSubmissionService : ICatalogSubmissionService
{
    private readonly CatalogStore _catalogStore;
    private readonly ObjectStorageService _objectStorage;
    public CatalogSubmissionService(
        CatalogStore catalogStore,
        ObjectStorageService objectStorage
    )
    {
        _catalogStore = catalogStore;
        _objectStorage = objectStorage;
    }

    public async Task<CatalogSubmission> CreateSubmissionAsync(
        CatalogSubmissionDto submissionDto,
        Guid submitterId
    )
    {
        string? imageKey = null;
        bool imageUploaded = false;
        if (submissionDto.CategoryId.HasValue)
        {
            var category = await _catalogStore.GetCategoryByIdAsync(
                submissionDto.CategoryId.Value
            );

            if (category == null)
            {
                throw new KeyNotFoundException("Категория не найдена");
            }
        }
        try
        {
            if (submissionDto.Image != null)
            {
                imageKey = $"submissions/{Guid.NewGuid():N}";
                using var stream = submissionDto.Image.OpenReadStream();  
                await _objectStorage.UploadAsync(
                    imageKey,
                    stream,
                    submissionDto.Image.ContentType
                );
                imageUploaded = true;
            }

            var submission = new CatalogSubmission
            {
                Title = submissionDto.Title.Trim(),
                Description = submissionDto.Description?.Trim(),
                CategoryId = submissionDto.CategoryId,
                ProposedCategoryName = submissionDto.ProposedCategoryName?.Trim(),
                SubmitterId = submitterId,
                Status = "pending",
                ImageKey = imageKey,
                CreatedAt = DateTime.UtcNow
            };

            _catalogStore.AddCatalogSubmission(submission);

            await _catalogStore.SaveAllChangesAsync();

            return submission;
        }
        catch (Exception)
        {
            if (imageUploaded && imageKey != null)
            {
                await _objectStorage.DeleteAsync(imageKey);
            }
            throw;
        }
    }

    public async Task<List<CatalogSubmissionItemDto>> GetMySubmissionsAsync(Guid submitterId)
    {
        var responseStore =  await _catalogStore
            .GetSubmissionsBySubmitterIdAsync(submitterId);
        var response = new List<CatalogSubmissionItemDto>();
        foreach (var submission in responseStore)
        {
            var submissionItem = new CatalogSubmissionItemDto
            {
                Id = submission.Id,
                Title = submission.Title,
                Description = submission.Description,
                CategoryId = submission.CategoryId,
                ProposedCategoryName = submission.ProposedCategoryName,
                Status = submission.Status,
                CreatedAt = submission.CreatedAt,
                ResultObjectId = submission.ResultObjectId,
                RejectionReason = submission.RejectionReason
            };
            if (submission.ImageKey != null)
            {
                submissionItem.ImageUrl = await _objectStorage
                    .GetPresignedUrl(submission.ImageKey, TimeSpan.FromMinutes(15));
            }
            response.Add(submissionItem);

        }
        return response;
    }
}
