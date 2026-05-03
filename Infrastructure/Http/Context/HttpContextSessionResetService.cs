using Microsoft.Extensions.Options;

namespace LevelUpLifeBackend.Infrastructure.Http.Context;

public sealed class HttpContextSessionResetService : ISessionResetService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly BaseHttpClientOptions _options;

    public HttpContextSessionResetService(
        IHttpContextAccessor httpContextAccessor,
        IOptions<BaseHttpClientOptions> options
    )
    {
        _httpContextAccessor = httpContextAccessor;
        _options = options.Value;
    }

    public Task ResetAsync(CancellationToken cancellationToken = default)
    {
        var context = _httpContextAccessor.HttpContext;
        if (context is null)
        {
            return Task.CompletedTask;
        }

        context.Response.Cookies.Delete(_options.SessionCookieName);
        return Task.CompletedTask;
    }
}
