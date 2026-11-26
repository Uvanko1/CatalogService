using CatalogService.DTOs;

namespace CatalogService.Services;

public class TestUserServiceClient : IUserServiceClient
{
    public Task<UserServiceDto> IntrospectAsync(string accessToken)
    {
        if (accessToken != "test-token")
        {
            return Task.FromResult(new UserServiceDto
            {
                Active = false,
                UserId = null
            });
        }

        return Task.FromResult(new UserServiceDto
        {
            Active = true,
            UserId = Guid.Parse("11111111-1111-1111-1111-111111111111")
        });
    }
}