
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CatalogService.DTOs;

public class CatalogSubmissionDto : IValidatableObject
{
    [Required(ErrorMessage = "Название обязательно")]
    public string Title { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "ID категории должен быть больше нуля")]
    public int? CategoryId { get; set; }

    public string? ProposedCategoryName { get; set; }

    [StringLength(5000, ErrorMessage = "Описание не должно превышать 5000 символов")]
    public string? Description { get; set; }

    public IFormFile? Image { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Title) ||
            Title.Trim().Length > 200)
        {
            yield return new ValidationResult(
                "Название должно содержать от 1 до 200 символов",
                [nameof(Title)]
            );
        }

        if (ProposedCategoryName is not null &&
            (string.IsNullOrWhiteSpace(ProposedCategoryName) ||
             ProposedCategoryName.Trim().Length > 100))
        {
            yield return new ValidationResult(
                "Название категории должно содержать от 1 до 100 символов",
                [nameof(ProposedCategoryName)]
            );
        }

        bool hasCategoryId = CategoryId.HasValue;
        bool hasProposedCategoryName = ProposedCategoryName is not null;

        if (hasCategoryId == hasProposedCategoryName)
        {
            yield return new ValidationResult(
                "Необходимо указать либо ID существующей категории, либо название новой категории",
                [
                    nameof(CategoryId),
                    nameof(ProposedCategoryName)
                ]
            );
        }
    }
}
