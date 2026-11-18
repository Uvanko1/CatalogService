namespace CatalogService.Services;
using GraphQL.Client;
using CatalogService.Configuration;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using GraphQL;
using CatalogService.DTOs;
using Microsoft.Extensions.Options;
using CatalogService.Models;

public class AnimeImportService
{
    private readonly ShikimoriApiOptions _shikimoriApiOptions;
    private readonly CatalogStore _catalogStore;
    private readonly ObjectStorageService _objectStorageService;
    private readonly HttpClient _httpClient;

    public AnimeImportService(
        IOptions<ShikimoriApiOptions> shikimoriApiOptions,
        CatalogStore catalogStore,
        ObjectStorageService objectStorageService,
        HttpClient httpClient
    )
    {
        _shikimoriApiOptions = shikimoriApiOptions.Value;
        _catalogStore = catalogStore;
        _objectStorageService = objectStorageService;
        _httpClient = httpClient;
    }

    public async Task ImportAnimeAsync()
    {
        var client = new GraphQLHttpClient(
            _shikimoriApiOptions.BaseUrl,
            new NewtonsoftJsonSerializer()
        );

        var request = new GraphQLRequest
        {
            Query = @"
            {
                  animes(limit: 20) {
                    name
                    russian
                    score

                    poster {
                        originalUrl
                    }

                    scoresStats {
                        count
                    }

                    description
                    }
                }"
        };
        var categories = await _catalogStore.GetCategories();
        var animeCategory = categories.First(x => x.NormalizedName == "anime").Id;
        var response = await client.SendQueryAsync<AnimesDto>(request);
        Console.WriteLine(response.Data.animes.First());
        foreach (var anime in response.Data.animes)
        {
            var catalogObject = new CatalogObject
            {
                Title = anime.russian,
                Description = anime.description,
                CategoryId = animeCategory
            };
            var animeId = await _catalogStore.AddCatalogObjectAsync(catalogObject);
            var imageKey = $"objects/anime/{animeId}.jpeg";
            using var responseImage = await _httpClient.GetAsync(anime.poster.originalUrl);
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
