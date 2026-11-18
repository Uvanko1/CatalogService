using CatalogService.Configuration;
using CatalogService.DTOs;
using CatalogService.Models;
using Microsoft.Extensions.Options;

namespace CatalogService.Services;

public class GamesImportService
{
    private readonly ObjectStorageService _objectStorageService;
    private readonly CatalogStore _catalogStore;
    private readonly SteamApiOptions _steamApiOptions;
    private readonly HttpClient _httpClient;

    public GamesImportService(
        ObjectStorageService objectStorageService,
        CatalogStore catalogStore,
        IOptions<SteamApiOptions> steamApiOptions,
        HttpClient httpClient
    )
    {
        _objectStorageService = objectStorageService;
        _catalogStore = catalogStore;
        _steamApiOptions = steamApiOptions.Value;
        _httpClient = httpClient;
    }

    public async Task ImportGamesAsync()
    {
        var categories = await _catalogStore.GetCategories();
        var gamesCategory = categories.First(x => x.NormalizedName == "games").Id;
        using var response = await _httpClient.GetAsync(_steamApiOptions.PopularGamesUrl);
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadFromJsonAsync<SteamResponseDto>();
        if (responseContent == null)
        {
            Console.WriteLine("Steam не отвечает");
            return;
        }
        foreach (var game in responseContent.response.ranks)
        {
            using var responseGame = await 
            _httpClient.GetAsync(_steamApiOptions.GameUrl + $"&appids={game.appid}");
            responseGame.EnsureSuccessStatusCode();
            var responseGameContent = 
                await responseGame.Content.ReadFromJsonAsync<Dictionary<string, SteamGameDetailsWrapperDto>>();
            if (responseGameContent == null)
            {
                Console.WriteLine("Игра не найдена");
                continue;
            }
            responseGameContent.TryGetValue(game.appid.ToString(), out var gameDetails);
            if (gameDetails == null)
            {
                Console.WriteLine("Такой игры нет");
                continue;
            }
            if (gameDetails.data == null)
            {
                Console.WriteLine("Данные не найдены");
                continue;
            }
            var catalogObject = new CatalogObject
            {
                Title = gameDetails.data.name,
                Description = gameDetails.data.short_description,
                CategoryId = gamesCategory
            };
            var gameId = await _catalogStore.AddCatalogObjectAsync(catalogObject);
            var imageKey = $"objects/games/{gameId}.jpg";
            using var responseImage = await _httpClient.GetAsync(gameDetails.data.header_image);
            responseImage.EnsureSuccessStatusCode();
            await using var imageStream = await responseImage.Content.ReadAsStreamAsync();
            await _objectStorageService.UploadAsync(
                imageKey,
                imageStream,
                "image/jpeg"
            );
            catalogObject.ImageKey = imageKey;
            await _catalogStore.UpdateCatalogObjectAsync(catalogObject);
        }
    }
}
