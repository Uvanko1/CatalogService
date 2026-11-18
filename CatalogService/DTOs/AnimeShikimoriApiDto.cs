namespace CatalogService.DTOs;

public class AnimeShikimoriApiDto
{
    public string name { get; set; } = string.Empty;
    public string russian { get; set; } = string.Empty;
    public OriginalUrlDto poster { get; set; } = new OriginalUrlDto();
    public List<RatingCountDto> scoresStats { get; set; } = new List<RatingCountDto>();
    public string description { get; set; } = string.Empty;
}
