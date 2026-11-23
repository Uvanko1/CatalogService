using CatalogService.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Data;

public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<CatalogObject> CatalogObjects => Set<CatalogObject>();

    public DbSet<CatalogSubmission> CatalogSubmissions => Set<CatalogSubmission>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasPostgresExtension("pg_trgm");

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("categories");

            entity.Property(x => x.NormalizedName)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(x => x.NormalizedName)
                .IsUnique();

            entity.OwnsOne(x => x.Capabilities, capability =>
            {
                capability.Property(x => x.SupportsStatus)
                    .HasColumnName("supports_status")
                    .HasDefaultValue(false);

                capability.Property(x => x.AllowedStatuses)
                    .HasColumnName("allowed_statuses")
                    .HasDefaultValueSql("ARRAY[]::text[]");
            });

            entity.Navigation(x => x.Capabilities).IsRequired();

            entity.HasMany(x => x.CatalogObjects)
                .WithOne(x => x.Category)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CatalogObject>(entity =>
        {
            entity.ToTable("catalog_objects");
            
            entity.HasIndex(x => x.Title, "IX_catalog_objects_title_trgm")
                .HasDatabaseName("ix_catalog_objects_title_trgm")
                .HasMethod("gin")
                .HasOperators("gin_trgm_ops");

            entity.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.Description)
                .HasMaxLength(5000);
        });

        modelBuilder.Entity<CatalogSubmission>(entity =>
        {
            entity.ToTable("catalog_submissions", table =>
            {
                table.HasCheckConstraint("CK_catalog_submissions_category_selection",
                    """
                    (("CategoryId" IS NOT NULL AND "ProposedCategoryName" IS NULL) OR
                    ("CategoryId" IS NULL AND "ProposedCategoryName" IS NOT NULL AND
                    length(btrim("ProposedCategoryName")) BETWEEN 1 AND 100))
                    """);
            });

            entity.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.Description)
                .HasMaxLength(5000);

            entity.Property(x => x.ProposedCategoryName)
                .HasMaxLength(100);

            entity.Property(x => x.Status)
                .IsRequired()
                .HasMaxLength(20);

            entity.HasOne<Category>()
                .WithMany()
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<CatalogObject>()
                .WithMany()
                .HasForeignKey(x => x.ResultObjectId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}