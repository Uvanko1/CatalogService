namespace CatalogService.DTOs;

public class BooksDto
{
    public string title { get; set; } = string.Empty;
    public string annotation { get; set; } = string.Empty;
    public string? cover_url { get; set; } = string.Empty;
}
