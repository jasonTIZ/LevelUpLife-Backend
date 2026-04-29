using System.Net.Http.Json;

namespace LevelUpLifeBackend.Infrastructure.Http;

public interface IBaseApiClient
{
    Task<HttpResponseMessage> GetAsync(string relativePath, CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> DeleteAsync(string relativePath, CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> PostAsJsonAsync<TBody>(
        string relativePath,
        TBody body,
        CancellationToken cancellationToken = default
    );
    Task<HttpResponseMessage> PutAsJsonAsync<TBody>(
        string relativePath,
        TBody body,
        CancellationToken cancellationToken = default
    );
    Task<HttpResponseMessage> PatchAsJsonAsync<TBody>(
        string relativePath,
        TBody body,
        CancellationToken cancellationToken = default
    );
}
