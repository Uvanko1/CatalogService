namespace CatalogService.Services;

using CatalogService.Data;

using CatalogService.DTOs;
using CatalogService.Models;
using Microsoft.EntityFrameworkCore;


public class CatalogStore
{
    private readonly CatalogDbContext CatalogDbContext;

    public CatalogStore(CatalogDbContext catalogDbContext)
    {
        CatalogDbContext = catalogDbContext;
    }

    public async Task<List<Category>> GetCategories()
    {
        return await CatalogDbContext.Categories.ToListAsync();
    }

    public async Task<List<CatalogObject>> GetObjects(
        string? query, int? categoryId, int page)
    {
        var objectsQuery = CatalogDbContext.CatalogObjects.AsQueryable();

        if (!string.IsNullOrEmpty(query))
        {
            objectsQuery = objectsQuery
                .Where(x => EF.Functions.ILike(x.Title, $"%{query}%"))
                .OrderBy(x =>
                    EF.Functions.ILike(x.Title, query) ? 0 :
                    EF.Functions.ILike(x.Title, $"{query}%") ? 1 : 2)
                    .ThenBy(x => x.Title);
        }
        else
        {
            objectsQuery = objectsQuery.OrderBy(x => x.Title);
        }
        if (categoryId.HasValue)
        {
            objectsQuery = objectsQuery.Where(x => x.CategoryId == categoryId.Value);
        }

        return await objectsQuery
            .Skip((page - 1) * 10)
            .Take(10)
            .ToListAsync();
    }

    public async Task<CatalogObject?> GetObjectByIdAsync(int id)
    {
        return await CatalogDbContext.CatalogObjects.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<int> AddCatalogObjectAsync(CatalogObject catalogObject)
    {
        CatalogDbContext.CatalogObjects.Add(catalogObject);
        await CatalogDbContext.SaveChangesAsync();
        return catalogObject.Id;
    }

    public async Task UpdateCatalogObjectAsync(CatalogObject catalogObject)
    {
        CatalogDbContext.CatalogObjects.Update(catalogObject);
        await CatalogDbContext.SaveChangesAsync();
    }
}
