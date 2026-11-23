using CatalogService.DTOs;

namespace CatalogService.Services;


public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception)
        {
            if (context.Response.HasStarted)
            {
                throw;
            }
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            var errorResponse = ErrorResponseFactory.Create(
                "INTERNAL_SERVER_ERROR",
                "Внутренняя ошибка сервера",
                context.TraceIdentifier
            );
            await context.Response.WriteAsJsonAsync(errorResponse);
        }
    }
}
