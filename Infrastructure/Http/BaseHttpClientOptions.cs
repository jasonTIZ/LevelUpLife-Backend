namespace LevelUpLifeBackend.Infrastructure.Http;

public sealed class BaseHttpClientOptions
{
    public const string SectionName = "ExternalApi";

    /// <summary>
    /// Absolute base URL of the downstream API (for example https://api.example.com).
    /// Required whenever <see cref="IBaseApiClient"/> is used: the typed HttpClient must have <see cref="System.Net.Http.HttpClient.BaseAddress"/> set from this value.
    /// </summary>
    public string? BaseUrl { get; set; }

    // Contract requires all calls to be prefixed by /v1.
    public string ApiPrefix { get; set; } = "/v1";

    public string SessionCookieName { get; set; } = "SESSION_ID";
}
