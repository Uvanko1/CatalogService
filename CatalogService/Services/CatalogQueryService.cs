namespace CatalogService.Services;
using CatalogService.DTOs;
using GraphQL;


public class CatalogQueryService
{
    private readonly CatalogStore _catalogStore;
    private readonly ObjectStorageService _objectStorageService;
    public CatalogQueryService(CatalogStore catalogStore, ObjectStorageService objectStorageService)
    {
        _catalogStore = catalogStore;
        _objectStorageService = objectStorageService;
    }

    public async Task<List<CategoryDto>> GetCategories()
    {
        var categories = await _catalogStore.GetCategories();
        var result = categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name
        }).ToList();
        return result;
    }
    public async Task<List<ObjectPreviewDto>> GetObjects(
        string? query, int? categoryId, int page)
    {
        var result = new List<ObjectPreviewDto>();
        var objects = await _catalogStore.GetObjects(query, categoryId, page);
        foreach (var obj in objects)
        {
            string? ImageUrl = null;
            if (obj.ImageKey != null)
            {
                ImageUrl = await _objectStorageService.GetPresignedUrl(obj.ImageKey, TimeSpan.FromMinutes(15));
            }
            
            result.Add(new ObjectPreviewDto
            {
                id = obj.Id,
                title = obj.Title,
                CategoryId = obj.CategoryId,
                ImageUrl = ImageUrl,
                RatingSummary = new RatingDto()
            });
        }
        return result;
    }

    public async Task<CatalogObjectDto?> GetObjectByIdAsync(int id)
    {
        var catalogObject = await _catalogStore.GetObjectByIdAsync(id);
        if (catalogObject == null)
        {
            return null;
        }
        string? ImageUrl = null;
        if (catalogObject.ImageKey != null)
        {
            ImageUrl = await _objectStorageService.GetPresignedUrl(catalogObject.ImageKey, TimeSpan.FromMinutes(15));
        }
        return new CatalogObjectDto
        {
            Id = catalogObject.Id,
            Title = catalogObject.Title,
            Description = catalogObject.Description,
            CategoryId = catalogObject.CategoryId,
            ImageUrl = ImageUrl
        };
    }
}
