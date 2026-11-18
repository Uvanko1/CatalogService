namespace CatalogService.Services;
using CatalogService.Configuration;
using CatalogService.DTOs;
using CatalogService.Models;
using Microsoft.Extensions.Options;


public class FilmsImportService
{
    private readonly CatalogStore _catalogStore;
    private readonly ObjectStorageService _objectStorageService;
    private readonly KinopoiskApiOptions _kinopoiskApiOptions;
    private readonly HttpClient _httpClient;

    public FilmsImportService(CatalogStore catalogStore,
        ObjectStorageService objectStorageService,
        IOptions<KinopoiskApiOptions> kinopoiskApiOptions,
        HttpClient httpClient)
    {
        _catalogStore = catalogStore;
        _objectStorageService = objectStorageService;
        _kinopoiskApiOptions = kinopoiskApiOptions.Value;
        _httpClient = httpClient;
    }

    public async Task ImportFilmsAsync()
    {
        var categories = await _catalogStore.GetCategories();
        var filmsCategory = categories.First(x => x.NormalizedName == "films").Id;
        _httpClient.DefaultRequestHeaders.Add("X-API-KEY", _kinopoiskApiOptions.ApiKey);
        using var response = await _httpClient.GetAsync(
            $"{_kinopoiskApiOptions.BaseUrl}/films/collections?type=TOP_POPULAR_MOVIES&page=1"
        );
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadFromJsonAsync<FilmsKinopoiskApiDto>();
        if (responseContent != null)
        {
            foreach (var item in responseContent.Items)
            {
                var catalogObject = new CatalogObject
                {
                    Title = item.nameRu,
                    Description = item.description,
                    CategoryId = filmsCategory
                };
                var filmId = await _catalogStore.AddCatalogObjectAsync(catalogObject);
                var imageKey = $"objects/films/{filmId}.jpg";
                using var responseImage = await _httpClient.GetAsync(item.posterUrl);
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
}
