using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace LevelUpLifeBackend.Infrastructure.Http;

public sealed class BaseApiClient : IBaseApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly HttpClient _httpClient;
    private readonly BaseHttpClientOptions _options;

    public BaseApiClient(HttpClient httpClient, IOptions<BaseHttpClientOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public Task<HttpResponseMessage> GetAsync(
        string relativePath,
        CancellationToken cancellationToken = default
    ) => _httpClient.GetAsync(BuildPath(relativePath), cancellationToken);

    public Task<HttpResponseMessage> DeleteAsync(
        string relativePath,
        CancellationToken cancellationToken = default
    ) => _httpClient.DeleteAsync(BuildPath(relativePath), cancellationToken);

    public Task<HttpResponseMessage> PostAsJsonAsync<TBody>(
        string relativePath,
        TBody body,
        CancellationToken cancellationToken = default
    ) => _httpClient.PostAsJsonAsync(BuildPath(relativePath), body, JsonOptions, cancellationToken);

    public Task<HttpResponseMessage> PutAsJsonAsync<TBody>(
        string relativePath,
        TBody body,
        CancellationToken cancellationToken = default
    ) => _httpClient.PutAsJsonAsync(BuildPath(relativePath), body, JsonOptions, cancellationToken);

    public Task<HttpResponseMessage> PatchAsJsonAsync<TBody>(
        string relativePath,
        TBody body,
        CancellationToken cancellationToken = default
    )
    {
        var request = new HttpRequestMessage(HttpMethod.Patch, BuildPath(relativePath))
        {
            Content = JsonContent.Create(body, options: JsonOptions),
        };

        return _httpClient.SendAsync(request, cancellationToken);
    }

    private string BuildPath(string relativePath)
    {
        var prefix = NormalizeSegment(_options.ApiPrefix);
        var path = NormalizeSegment(relativePath);
        return $"{prefix}/{path}";
    }

    private static string NormalizeSegment(string value)
    {
        var normalized = value.Trim();
        normalized = normalized.Trim('/');
        return normalized;
    }
}
