namespace CatalogService.DTOs;

public class SteamGameDetailsResponseDto
{
    public Dictionary<string, SteamGameDetailsWrapperDto> Games { get; set; } = new();
}
