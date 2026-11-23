using CatalogService.Configuration;

namespace CatalogService.DTOs;

public class ObjectPreviewDto
{
    public int id { get; set; }
    public string title { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string? ImageUrl { get; set; } = null;
    public RatingDto? RatingSummary { get; set; } = null;
    public RatingStatus RatingSummaryStatus { get; set; } = RatingStatus.Available;
}
