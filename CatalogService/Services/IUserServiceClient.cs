namespace CatalogService.Services;
using CatalogService.DTOs;

public interface IUserServiceClient
{
    Task<UserServiceDto> IntrospectAsync(string accessToken);
}
