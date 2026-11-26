using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using CatalogService.Data;
using CatalogService.Services;
using CatalogService.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy =
            System.Text.Json.JsonNamingPolicy.SnakeCaseLower;
        
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter(
                System.Text.Json.JsonNamingPolicy.SnakeCaseLower
            )
        );
    });
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
builder.Services.Configure<UserServiceOptions>(
    builder.Configuration.GetSection("UserService")
);
builder.Services.AddHttpClient<FilmsImportService>();
builder.Services.AddHttpClient<AnimeImportService>();
builder.Services.AddHttpClient<GamesImportService>();
builder.Services.AddHttpClient<BooksImportService>();
builder.Services.AddScoped<IUserServiceClient, TestUserServiceClient>();
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
builder.Services.AddScoped<CatalogQueryService>();
builder.Services.AddScoped<ICatalogSubmissionService, CatalogSubmissionService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Введите тестовый токен: test-token"
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
});

builder.Services.AddDbContext<CatalogDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("CatalogDatabase")));

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
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

