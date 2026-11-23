    using System.Net.Http.Headers;
    using CatalogService.Configuration;
    using CatalogService.DTOs;
    using Microsoft.Extensions.Options;

    namespace CatalogService.Services;

    public class UserServiceClient: IUserServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly UserServiceOptions _userServiceOptions;
        public UserServiceClient(
            HttpClient httpClient, 
            IOptions<UserServiceOptions> userServiceOptions
        )
        {
            _httpClient = httpClient;
            _userServiceOptions = userServiceOptions.Value;
            _httpClient.BaseAddress = new Uri(_userServiceOptions.BaseUrl);
        }
        public async Task<UserServiceDto> IntrospectAsync(string accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new ArgumentException("Некорректный токен", "accessToken");
            }
            using var message = new HttpRequestMessage(
                HttpMethod.Post,
                _userServiceOptions.IntrospectEndpoint
            );
            message.Headers.Authorization = 
                new AuthenticationHeaderValue("Bearer", accessToken);
            using var response = await _httpClient.SendAsync(message);
            response.EnsureSuccessStatusCode();
            var responseData = await response.Content.ReadFromJsonAsync<UserServiceDto>();
            if (responseData == null)
            {
                throw new InvalidOperationException("Ответ сервиса пустой");
            }
            return responseData;
        }
    }
