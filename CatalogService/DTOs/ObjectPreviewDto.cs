using CatalogService.Configuration;

namespace CatalogService.DTOs;

public class ObjectPreviewDto
{
    public int id { get; set; }
    public string title { get; set; } = string.Empty;
    public int category_id { get; set; }
    public string? image_url { get; set; } = null;
    public RatingDto rating_summary { get; set; } = new RatingDto();
    public RatingStatus rating_summary_status { get; set; } = RatingStatus.available;
}
