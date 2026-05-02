using System.Net.Http.Headers;
using LevelUpLifeBackend.Infrastructure.Http.Context;
using Microsoft.Extensions.Options;

namespace LevelUpLifeBackend.Infrastructure.Http.Handlers;

public sealed class AuthSessionHandler : DelegatingHandler
{
    private readonly ISecureCredentialStorage _credentialStorage;
    private readonly BaseHttpClientOptions _options;

    public AuthSessionHandler(
        ISecureCredentialStorage credentialStorage,
        IOptions<BaseHttpClientOptions> options
    )
    {
        _credentialStorage = credentialStorage;
        _options = options.Value;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        var credentials = _credentialStorage.GetCredentials();
        
        if (credentials != null)
        {
            if (!string.IsNullOrWhiteSpace(credentials.Jwt))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", credentials.Jwt);
            }

            if (!string.IsNullOrWhiteSpace(credentials.SessionId))
            {
                // Attach SESSION_ID as a Cookie header
                request.Headers.Add("Cookie", $"{_options.SessionCookieName}={credentials.SessionId}");
            }
        }

        return base.SendAsync(request, cancellationToken);
    }
}
