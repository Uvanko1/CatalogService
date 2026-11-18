namespace CatalogService.DTOs;

public class KinopoiskApiDto
{
    public int kinopoiskId { get; set; }
    public string? imdbId { get; set; } = null;
    public string nameRu { get; set; } = string.Empty;
    public string? nameEn { get; set; } = null;
    public string? nameOriginal { get; set; } = null;
    public List<CountryDto> countries { get; set; } = new List<CountryDto>();
    public List<GenreDto> genres { get; set; } = new List<GenreDto>();
    public decimal? ratingKinopoisk { get; set; } = null;
    public float? ratingImdb { get; set; } = null;
    public int year { get; set; }
    public string type { get; set; } = string.Empty;
    public string posterUrl { get; set; } = string.Empty;
    public string posterUrlPreview { get; set; } = string.Empty;
    public string? coverUrl { get; set; } = null;
    public string? logoUrl { get; set; } = null;
    public string description { get; set; } = string.Empty;
    public string? ratingAgeLimits { get; set; } = null;
}
