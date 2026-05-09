using Microsoft.Extensions.Options;

namespace LevelUpLifeBackend.Infrastructure.Http.Context;

public sealed class HttpContextSessionResetService : ISessionResetService
{
    private readonly ISecureCredentialStorage _credentialStorage;
    private readonly BaseHttpClientOptions _options;

    public HttpContextSessionResetService(
        ISecureCredentialStorage credentialStorage,
        IOptions<BaseHttpClientOptions> options
    )
    {
        _credentialStorage = credentialStorage;
        _options = options.Value;
    }

    public Task ResetAsync(CancellationToken cancellationToken = default)
    {
        _credentialStorage.ClearCredentials();
        return Task.CompletedTask;
    }
}
