using System.Net.Http.Headers;
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
        var context = _httpContextAccessor.HttpContext;
        if (context is null)
        {
            return null;
        }

        var values = context.Request.Headers.Authorization;
        if (values.Count == 0)
        {
            return null;
        }

        // Use the first value (StringValues.ToString() can join multiples with commas).
        var raw = values[0];
        if (string.IsNullOrWhiteSpace(raw) || !AuthenticationHeaderValue.TryParse(raw, out var parsed))
        {
            return null;
        }

        if (!string.Equals(parsed.Scheme, "Bearer", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return string.IsNullOrWhiteSpace(parsed.Parameter) ? null : parsed.Parameter;
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
