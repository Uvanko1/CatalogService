namespace CatalogService.DTOs;

public class SteamDataDto
{
    public int rollup_date { get; set; }
    public List<GamePositionDto> ranks { get; set; } = new List<GamePositionDto>();
}
