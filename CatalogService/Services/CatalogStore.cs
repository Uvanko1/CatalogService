namespace CatalogService.Services;

using CatalogService.Data;

using CatalogService.DTOs;
using CatalogService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

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
            query = EscapeLikePattern(query);
            objectsQuery = objectsQuery
                .Where(x => EF.Functions.ILike(x.Title, $"%{query}%", "\\"))
                .OrderBy(x =>
                    EF.Functions.ILike(x.Title, query, "\\") ? 0 :
                    EF.Functions.ILike(x.Title, $"{query}%", "\\") ? 1 : 2)
                    .ThenBy(x => x.Title.ToLower()).ThenBy(x => x.Id);
        }
        else
        {
            objectsQuery = objectsQuery.OrderBy(x => x.Title.ToLower()).ThenBy(x => x.Id);
        }
        if (categoryId.HasValue)
        {
            objectsQuery = objectsQuery.Where(x => x.CategoryId == categoryId.Value);
        }

        Console.WriteLine(objectsQuery.ToQueryString());

        return await objectsQuery
            .Skip((page - 1) * 20)
            .Take(20)
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

    private static string EscapeLikePattern(string query)
    {
        return query
            .Replace("\\", "\\\\")
            .Replace("%", "\\%")
            .Replace("_", "\\_");
    }

    public async Task<CatalogSubmission?> GetSubmissionByIdAsync(int id)
    {
        return await CatalogDbContext.CatalogSubmissions
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<CatalogSubmission?> GetSubmissionForUpdateAsync(int id)
    {
        return await CatalogDbContext.CatalogSubmissions
            .FromSqlInterpolated(
                $"SELECT * FROM catalog_submissions WHERE \"Id\" = {id} FOR UPDATE"
            )
            .FirstOrDefaultAsync();
    }

    public async Task<Category?> GetCategoryByNormalizedNameAsync(string normalizedName)
    {
        return await CatalogDbContext.Categories
            .FirstOrDefaultAsync(x => x.NormalizedName == normalizedName);
    }

    public async Task<Category?> GetCategoryByIdAsync(int id)
    {
        return await CatalogDbContext.Categories
            .FirstOrDefaultAsync(x => x.Id == id);
    }
    public async Task<SubmissionDecision?> GetSubmissionDecisionAsync(int submissionId)
    {
        return await CatalogDbContext.SubmissionDecisions
            .FirstOrDefaultAsync(x => x.SubmissionId == submissionId);
    }

    public void AddCategory(Category category)
    {
        CatalogDbContext.Categories.Add(category);
    }

    public void AddCatalogObject(CatalogObject catalogObject)
    {
        CatalogDbContext.CatalogObjects.Add(catalogObject);
    }

    public void AddSubmissionDecision(SubmissionDecision submissionDecision)
    {
        CatalogDbContext.SubmissionDecisions.Add(submissionDecision);
    }

    public async Task SaveAllChangesAsync()
    {
        await CatalogDbContext.SaveChangesAsync();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await CatalogDbContext.Database.BeginTransactionAsync();
    }

    public void AddCatalogSubmission(CatalogSubmission submission)
    {
        CatalogDbContext.CatalogSubmissions.Add(submission);
    }

    public async Task<List<CatalogSubmission>> GetSubmissionsBySubmitterIdAsync(
        Guid submitterId
    )
    {
        return await CatalogDbContext.CatalogSubmissions
            .Where(s => s.SubmitterId == submitterId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }
}
