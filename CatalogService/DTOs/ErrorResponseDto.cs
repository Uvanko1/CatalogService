namespace CatalogService.DTOs;

public class ErrorResponseDto
{
    public ErrorDto error { get; set; } = new ErrorDto();
    public string request_id { get; set; } = string.Empty;
}
