namespace CatalogService.Configuration;

public class UserServiceOptions
{
    public string BaseUrl { get; set; } = string.Empty;
    public string IntrospectEndpoint { get; set; } = "/internal/auth/introspect";
}
