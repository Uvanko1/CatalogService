using CatalogService.DTOs;
using CatalogService.Models;
using CatalogService.Models.Enums;
using System.Text.RegularExpressions;

namespace CatalogService.Services;

public class SubmissionDecisionService : ISubmissionDecisionService
{
    private readonly CatalogStore _catalogStore;

    public SubmissionDecisionService(CatalogStore catalogStore)
    {
        _catalogStore = catalogStore;
    }

    public async Task<SubmissionDecision> ApplyDecisionAsync(
        int submissionId,
        SubmissionDecisionDto submissionDecision
    )
    {
        Category? category = null;
        SubmissionDecision decision;
        CatalogObject? newObject = null;
        ValidateDecision(submissionDecision);
        await using var transaction = await _catalogStore.BeginTransactionAsync();
        try
        {
            var submission = await _catalogStore.GetSubmissionForUpdateAsync(submissionId);

            if (submission == null)
            {
                throw new KeyNotFoundException("Заявка не найдена");
            }

            var existingDecision = await _catalogStore.GetSubmissionDecisionAsync(submissionId);

            if (existingDecision != null)
            {
                if (submissionDecision.OperationId == existingDecision.OperationId)
                {
                    await transaction.CommitAsync();
                    return existingDecision;
                }

                throw new InvalidOperationException(
                    "Заявка уже обработана другой операцией"
                );
            }
            if (submissionDecision.Action == SubmissionDecisionAction.Approve)
            {
                int? categoryId;
                string? categoryName;

                if (submissionDecision.ResolvedCategoryId.HasValue ||
                    submissionDecision.ResolvedCategoryName is not null)
                {
                    categoryId = submissionDecision.ResolvedCategoryId;
                    categoryName = submissionDecision.ResolvedCategoryName;
                }
                else
                {
                    categoryId = submission.CategoryId;
                    categoryName = submission.ProposedCategoryName;
                }
                if (!string.IsNullOrWhiteSpace(categoryName))
                {
                    var normalizedName = Regex.Replace(
                        categoryName.Trim().ToLowerInvariant(),
                        @"\s+",
                        "_"
                    );
                    var categoryExist = await _catalogStore
                        .GetCategoryByNormalizedNameAsync(normalizedName);
                    if (categoryExist == null)
                    {
                        category = new Category
                        {
                            Name = categoryName.Trim(),
                            NormalizedName = normalizedName
                        };
                        _catalogStore.AddCategory(category);
                    }
                    else
                    {
                        category = categoryExist;
                    }
                }
                else if (categoryId.HasValue)
                {
                    var existingCategory = await _catalogStore
                        .GetCategoryByIdAsync(categoryId.Value);
                    if (existingCategory == null)
                    {
                        throw new KeyNotFoundException("Категория не найдена");
                    }

                    category = existingCategory;
                }
                else
                {
                    throw new ArgumentException("Необходимо указать категорию");
                }
                newObject = new CatalogObject
                {
                    Title = submission.Title,
                    Description = submission.Description,
                    ImageKey = submission.ImageKey,
                    Category = category
                };
                _catalogStore.AddCatalogObject(newObject);
                submission.Status = "approved";
            }
            else if (submissionDecision.Action == SubmissionDecisionAction.Reject)
            {
                submission.Status = "rejected";
                submission.RejectionReason = submissionDecision.RejectionReason;
            }
            decision = new SubmissionDecision
            {
                OperationId = submissionDecision.OperationId,
                SubmissionId = submissionId,
                Action = submissionDecision.Action,
                ResolvedCategoryId = submissionDecision.ResolvedCategoryId,
                ResolvedCategoryName = submissionDecision.ResolvedCategoryName,
                ResultCategoryId = 
                    submissionDecision.Action == SubmissionDecisionAction.Approve 
                    ? category?.Id : null,
                CreatedAt = DateTime.UtcNow
            };
            _catalogStore.AddSubmissionDecision(decision);
            await _catalogStore.SaveAllChangesAsync();
            if (submissionDecision.Action == SubmissionDecisionAction.Approve)
            {
                submission.ResultObjectId = newObject?.Id;
                decision.ResultObjectId = newObject?.Id;
                await _catalogStore.SaveAllChangesAsync();
            }
            await transaction.CommitAsync();
            return decision;
        }
        catch
        {
             await transaction.RollbackAsync();
            throw;
        }
    }

    private static void ValidateDecision(SubmissionDecisionDto decision)
    {
        if (decision.OperationId == Guid.Empty)
        {
            throw new ArgumentException("OperationId не может быть пустым");
        }

        if (!Enum.IsDefined(typeof(SubmissionDecisionAction), decision.Action))
        {
            throw new ArgumentException("Недопустимое действие");
        }

        if (decision.Action == SubmissionDecisionAction.Approve)
        {
            bool hasCategoryId = decision.ResolvedCategoryId.HasValue;
            bool hasCategoryName = decision.ResolvedCategoryName is not null;

            if (hasCategoryId && hasCategoryName)
            {
                throw new ArgumentException(
                    "Нельзя одновременно указывать ID и название категории"
                );
            }

            if (hasCategoryId && decision.ResolvedCategoryId <= 0)
            {
                throw new ArgumentException("ID категории должен быть больше нуля");
            }

            if (hasCategoryName)
            {
                if (string.IsNullOrWhiteSpace(decision.ResolvedCategoryName))
                {
                    throw new ArgumentException(
                        "Название категории не может быть пустым"
                    );
                }

                if (decision.ResolvedCategoryName.Trim().Length > 100)
                {
                    throw new ArgumentException(
                        "Название категории не должно превышать 100 символов"
                    );
                }
            }
        }
        else if (decision.Action == SubmissionDecisionAction.Reject)
        {
            if (string.IsNullOrWhiteSpace(decision.RejectionReason))
            {
                throw new ArgumentException(
                    "При отклонении заявки необходимо указать причину"
                );
            }

            if (decision.ResolvedCategoryId.HasValue ||
                decision.ResolvedCategoryName is not null)
            {
                throw new ArgumentException(
                    "При отклонении заявки нельзя указывать категорию"
                );
            }
        }
    }
}
