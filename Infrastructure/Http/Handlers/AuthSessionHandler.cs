using System.Net.Http.Headers;
using LevelUpLifeBackend.Infrastructure.Http.Context;
using Microsoft.Extensions.Options;

namespace LevelUpLifeBackend.Infrastructure.Http.Handlers;

public sealed class AuthSessionHandler : DelegatingHandler
{
    private readonly IRequestCredentialAccessor _credentialAccessor;
    private readonly BaseHttpClientOptions _options;

    public AuthSessionHandler(
        IRequestCredentialAccessor credentialAccessor,
        IOptions<BaseHttpClientOptions> options
    )
    {
        _credentialAccessor = credentialAccessor;
        _options = options.Value;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        var bearerToken = _credentialAccessor.GetBearerToken();
        if (!string.IsNullOrWhiteSpace(bearerToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        }

        var sessionId = _credentialAccessor.GetSessionId();
        if (!string.IsNullOrWhiteSpace(sessionId))
        {
            request.Headers.Remove(_options.SessionCookieName);
            request.Headers.Add(_options.SessionCookieName, sessionId);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
