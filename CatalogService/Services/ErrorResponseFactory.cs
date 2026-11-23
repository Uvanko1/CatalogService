using CatalogService.DTOs;

namespace CatalogService.Services;

public class ErrorResponseFactory
{
    public static ErrorResponseDto Create(
        string code,
        string message,
        string requestId
    )
    {
        return new ErrorResponseDto
        {
            error = new ErrorDto
            {
                code = code,
                message = message
            },
            request_id = requestId
        };
    }
}
