using Microsoft.AspNetCore.Mvc;
using CatalogService.DTOs;
using CatalogService.Services;
using CatalogService.Models;

namespace CatalogService.Controllers;


[ApiController]
[Route("/[controller]")]
public class CatalogController: ControllerBase
{
    private readonly CatalogQueryService CatalogQueryService;
    private readonly FilmsImportService _filmsImportService;
    private readonly AnimeImportService _animeImportService;
    private readonly GamesImportService _gamesImportService;
    private readonly BooksImportService _booksImportService;
    public CatalogController(
        CatalogQueryService catalogQueryService,
        FilmsImportService filmsImportService,
        AnimeImportService animeImportService,
        GamesImportService gamesImportService,
        BooksImportService booksImportService
    )
    {
        CatalogQueryService = catalogQueryService;
        _filmsImportService = filmsImportService;
        _animeImportService = animeImportService;
        _gamesImportService = gamesImportService;
        _booksImportService = booksImportService;
    }

    [Route("categories")]
    [HttpGet]
    public async Task<ActionResult<CategoriesResponseDto>> GetCategories()
    {
        var response = new CategoriesResponseDto
        {
            Items = await CatalogQueryService.GetCategories()
        };
        return Ok(response);
    }

    [Route("objects")]
    [HttpGet]
    public async Task<ActionResult<List<ObjectPreviewDto>>> GetObjects(
        [FromQuery] string? q,
        [FromQuery] int? category_id,
        [FromQuery] int page = 1)
    {
        if (!string.IsNullOrWhiteSpace(q) && category_id.HasValue)
        {
            return BadRequest(
                ErrorResponseFactory.Create(
                    "CONFLICTING_FILTERS",
                    "Параметры q и category_id нельзя использовать одновременно",
                    HttpContext.TraceIdentifier
                )
            );
        }
        if (category_id < 1 && category_id.HasValue)
        {
            return BadRequest(
                ErrorResponseFactory.Create(
                    "ILLEGAL_ARGUMENT_VALUE",
                    "Некорректное значение для категории",
                    HttpContext.TraceIdentifier
                )
            );
        }
        if (page < 1 || page > 1000)
        {
            return BadRequest(
                ErrorResponseFactory.Create(
                    "ILLEGAL_ARGUMENT_VALUE",
                    "Некорректное значение для страницы",
                    HttpContext.TraceIdentifier
                )
            );
        }
        string? query = null;
        if (!string.IsNullOrWhiteSpace(q))
        {
            query = q.Trim();
            if (query.Length < 2)
            {
                return BadRequest(
                    ErrorResponseFactory.Create(
                        "QUERY_TOO_SHORT",
                        "Слишком маленькая длина поискового запроса",
                        HttpContext.TraceIdentifier
                    )
                );
            }
        }
        
        var objects = await CatalogQueryService.GetObjects(query, category_id, page);
        return Ok(objects);
    }

    [Route("objects/{id}")]
    [HttpGet]
    public async Task<ActionResult<CatalogObjectDto>> GetObject(int id)
    {
        if (id < 1)
        {
            return BadRequest(
                ErrorResponseFactory.Create(
                    "ILLEGAL_ARGUMENT_VALUE",
                    "Некорректное значение для идентификатора объекта",
                    HttpContext.TraceIdentifier
                )
            );
        }
        var catalogObject = await CatalogQueryService.GetObjectByIdAsync(id);
        if (catalogObject == null)
        {
            return NotFound(
                ErrorResponseFactory.Create(
                    "NOT_FOUND",
                    "Объект каталога не найден",
                    HttpContext.TraceIdentifier
                )
            );
        }
        return Ok(catalogObject);
    }

    [Route("objects/films/import")]
    [HttpPost]
    public async Task<IActionResult> ImportFilms()
    {
        await _filmsImportService.ImportFilmsAsync();
        return Ok();
    }

    [Route("objects/anime/import")]
    [HttpPost]
    public async Task<IActionResult> ImportAnime()
    {
        await _animeImportService.ImportAnimeAsync();
        return Ok();
    }

    [Route("objects/games/import")]
    [HttpPost]
    public async Task<IActionResult> ImportGames()
    {
        await _gamesImportService.ImportGamesAsync();
        return Ok();
    }

    [Route("objects/books/import")]
    [HttpPost]
    public async Task<IActionResult> ImportBooks()
    {
        await _booksImportService.ImportBooksAsync();
        return Ok();
    }
}
