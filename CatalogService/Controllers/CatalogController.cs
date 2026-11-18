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
    public async Task<ActionResult<List<CategoryDto>>> GetCategories()
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
        var objects = await CatalogQueryService.GetObjects(q, category_id, page);
        if (objects == null)
        {
            return NotFound();
        }
        return Ok(objects);
    }

    [Route("objects/{id}")]
    [HttpGet]
    public async Task<ActionResult<CatalogObject>> GetObject(int id)
    {
        var catalogObject = await CatalogQueryService.GetObjectByIdAsync(id);
        if (catalogObject == null)
        {
            return NotFound();
        }
        return Ok(catalogObject);
    }

    [Route("objects/films/import")]
    [HttpGet]
    public async Task<IActionResult> ImportFilms()
    {
        await _filmsImportService.ImportFilmsAsync();
        return Ok();
    }

    [Route("objects/anime/import")]
    [HttpGet]
    public async Task<IActionResult> ImportAnime()
    {
        await _animeImportService.ImportAnimeAsync();
        return Ok();
    }

    [Route("objects/games/import")]
    [HttpGet]
    public async Task<IActionResult> ImportGames()
    {
        await _gamesImportService.ImportGamesAsync();
        return Ok();
    }

    [Route("objects/books/import")]
    [HttpGet]
    public async Task<IActionResult> ImportBooks()
    {
        await _booksImportService.ImportBooksAsync();
        return Ok();
    }
}
