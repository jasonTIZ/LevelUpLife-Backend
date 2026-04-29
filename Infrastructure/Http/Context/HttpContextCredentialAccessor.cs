using Microsoft.Extensions.Options;

namespace LevelUpLifeBackend.Infrastructure.Http.Context;

public sealed class HttpContextCredentialAccessor : IRequestCredentialAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly BaseHttpClientOptions _options;

    public HttpContextCredentialAccessor(
        IHttpContextAccessor httpContextAccessor,
        IOptions<BaseHttpClientOptions> options
    )
    {
        _httpContextAccessor = httpContextAccessor;
        _options = options.Value;
    }

    public string? GetBearerToken()
    {
        var authorizationHeader = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
        if (string.IsNullOrWhiteSpace(authorizationHeader))
        {
            return null;
        }

        const string bearerPrefix = "Bearer ";
        if (!authorizationHeader.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var token = authorizationHeader[bearerPrefix.Length..].Trim();
        return string.IsNullOrWhiteSpace(token) ? null : token;
    }

    public string? GetSessionId()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context is null)
        {
            return null;
        }

        var cookieValue = context.Request.Cookies[_options.SessionCookieName];
        if (!string.IsNullOrWhiteSpace(cookieValue))
        {
            return cookieValue;
        }

        var headerValue = context.Request.Headers[_options.SessionCookieName].ToString();
        return string.IsNullOrWhiteSpace(headerValue) ? null : headerValue;
    }
}
