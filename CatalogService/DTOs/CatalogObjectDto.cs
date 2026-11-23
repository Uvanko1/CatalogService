namespace CatalogService.DTOs;
using CatalogService.Configuration;

public class CatalogObjectDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; } = null;
    public int CategoryId { get; set; }
    public string? ImageUrl { get; set; } = null;
    public RatingDto? RatingSummary { get; set; } = null;
    public RatingStatus RatingSummaryStatus { get; set; } = RatingStatus.Available;
}
