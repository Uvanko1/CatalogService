namespace CatalogService.DTOs;

public class SteamGameDetailsWrapperDto
{
    public bool success { get; set; }
    public SteamGameDetailsDto? data { get; set; }
}
