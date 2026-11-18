using CatalogService.Configuration;
using CatalogService.DTOs;
using CatalogService.Models;
using Microsoft.Extensions.Options;

namespace CatalogService.Services;

public class BooksImportService
{
    private readonly ObjectStorageService _objectStorageService;
    private readonly CatalogStore _catalogStore;
    private readonly ReBooksApiOptions _reBooksApiOptions;
    private readonly HttpClient _httpClient;

    public BooksImportService(
        ObjectStorageService objectStorageService,
        CatalogStore catalogStore,
        IOptions<ReBooksApiOptions> reBooksApiOptions,
        HttpClient httpClient
    )
    {
        _objectStorageService = objectStorageService;
        _catalogStore = catalogStore;
        _reBooksApiOptions = reBooksApiOptions.Value;
        _httpClient = httpClient;
    }

    public async Task ImportBooksAsync()
    {
        var categories = await _catalogStore.GetCategories();
        var booksCategory = categories.First(x => x.NormalizedName == "books").Id;
        _httpClient.DefaultRequestHeaders.Add("x-api-key", _reBooksApiOptions.ApiKey);
        using var response = await _httpClient.GetAsync(_reBooksApiOptions.BaseUrl);
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadFromJsonAsync<BooksDataDto>();
        if (responseContent != null)
        {
            foreach (var book in responseContent.data.books)
            {
                var catalogObject = new CatalogObject
                {
                    Title = book.title,
                    Description = book.annotation,
                    CategoryId = booksCategory
                };
                if (book.cover_url == null)
                {
                    continue;
                }
                var bookId = await _catalogStore.AddCatalogObjectAsync(catalogObject);
                var imageKey = $"objects/books/{bookId}.jpg";
                using var responseImage = await _httpClient.GetAsync(book.cover_url);
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
