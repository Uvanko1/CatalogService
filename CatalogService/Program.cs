using Microsoft.EntityFrameworkCore;
using CatalogService.Data;
using CatalogService.Services;
using CatalogService.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddScoped<CatalogStore>();
builder.Services.Configure<ObjectStorageOptions>(
    builder.Configuration.GetSection("ObjectStorage")
);
builder.Services.Configure<KinopoiskApiOptions>(
    builder.Configuration.GetSection("KinopoiskApi")
);
builder.Services.Configure<ShikimoriApiOptions>(
    builder.Configuration.GetSection("ShikimoriApi")
);
builder.Services.Configure<SteamApiOptions>(
    builder.Configuration.GetSection("SteamApi")
);
builder.Services.Configure<ReBooksApiOptions>(
    builder.Configuration.GetSection("ReBooksApi")
);
builder.Services.AddHttpClient<FilmsImportService>();
builder.Services.AddHttpClient<AnimeImportService>();
builder.Services.AddHttpClient<GamesImportService>();
builder.Services.AddHttpClient<BooksImportService>();
builder.Services.AddSingleton<Amazon.S3.IAmazonS3, Amazon.S3.AmazonS3Client>(sp => 
    {
        var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<ObjectStorageOptions>>().Value;
        return new Amazon.S3.AmazonS3Client(
            new Amazon.Runtime.BasicAWSCredentials(
                options.AccessKeyId,
                options.SecretAccessKey
            ),
            new Amazon.S3.AmazonS3Config
            {
                ServiceURL = options.ServiceUrl,
                AuthenticationRegion = options.Region,
                ForcePathStyle = true
            }
        );
    }
);
builder.Services.AddScoped<ObjectStorageService>();
builder.Services.AddScoped<FilmsImportService>();
builder.Services.AddScoped<AnimeImportService>();
builder.Services.AddScoped<GamesImportService>();
builder.Services.AddScoped<BooksImportService>();
builder.Services.AddScoped<CatalogQueryService>();
builder.Services.AddDbContext<CatalogDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("CatalogDatabase")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();
// app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    var dbContext = scope.ServiceProvider
        .GetRequiredService<CatalogDbContext>();

    await dbContext.Database.MigrateAsync();
}


app.Run();

