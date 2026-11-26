using CatalogService.Models;

namespace CatalogService.DTOs;

public class CatalogSubmissionsDto
{
    public List<CatalogSubmissionItemDto> Items { get; set; } = new();
}
