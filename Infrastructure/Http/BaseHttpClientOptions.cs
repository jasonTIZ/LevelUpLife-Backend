namespace LevelUpLifeBackend.Infrastructure.Http;

public sealed class BaseHttpClientOptions
{
    public const string SectionName = "ExternalApi";

    // Optional absolute URL of downstream API (e.g. https://api.leveluplife.com).
    public string? BaseUrl { get; set; }

    // Contract requires all calls to be prefixed by /v1.
    public string ApiPrefix { get; set; } = "/v1";

    public string SessionCookieName { get; set; } = "SESSION_ID";
}
