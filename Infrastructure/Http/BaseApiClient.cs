using System.Net.Http.Json;
using System.Text.Json;
using LevelUpLifeBackend.Infrastructure.Errors;
using Microsoft.Extensions.Options;

namespace LevelUpLifeBackend.Infrastructure.Http;

public sealed class BaseApiClient : IBaseApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly HttpClient _httpClient;
    private readonly BaseHttpClientOptions _options;
    private readonly IApiErrorParser _apiErrorParser;

    public BaseApiClient(
        HttpClient httpClient,
        IOptions<BaseHttpClientOptions> options,
        IApiErrorParser apiErrorParser
    )
    {
        _httpClient = httpClient;
        _options = options.Value;
        _apiErrorParser = apiErrorParser;
    }

    public async Task<HttpResponseMessage> GetAsync(
        string relativePath,
        CancellationToken cancellationToken = default
    )
    {
        var response = await _httpClient
            .GetAsync(BuildPath(relativePath), cancellationToken)
            .ConfigureAwait(false);
        await EnsureSuccessAsync(response, cancellationToken).ConfigureAwait(false);
        return response;
    }

    public async Task<HttpResponseMessage> DeleteAsync(
        string relativePath,
        CancellationToken cancellationToken = default
    )
    {
        var response = await _httpClient
            .DeleteAsync(BuildPath(relativePath), cancellationToken)
            .ConfigureAwait(false);
        await EnsureSuccessAsync(response, cancellationToken).ConfigureAwait(false);
        return response;
    }

    public async Task<HttpResponseMessage> PostAsJsonAsync<TBody>(
        string relativePath,
        TBody body,
        CancellationToken cancellationToken = default
    )
    {
        var response = await _httpClient
            .PostAsJsonAsync(BuildPath(relativePath), body, JsonOptions, cancellationToken)
            .ConfigureAwait(false);
        await EnsureSuccessAsync(response, cancellationToken).ConfigureAwait(false);
        return response;
    }

    public async Task<HttpResponseMessage> PutAsJsonAsync<TBody>(
        string relativePath,
        TBody body,
        CancellationToken cancellationToken = default
    )
    {
        var response = await _httpClient
            .PutAsJsonAsync(BuildPath(relativePath), body, JsonOptions, cancellationToken)
            .ConfigureAwait(false);
        await EnsureSuccessAsync(response, cancellationToken).ConfigureAwait(false);
        return response;
    }

    public async Task<HttpResponseMessage> PatchAsJsonAsync<TBody>(
        string relativePath,
        TBody body,
        CancellationToken cancellationToken = default
    )
    {
        var request = new HttpRequestMessage(HttpMethod.Patch, BuildPath(relativePath))
        {
            Content = JsonContent.Create(body, options: JsonOptions),
        };

        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        await EnsureSuccessAsync(response, cancellationToken).ConfigureAwait(false);
        return response;
    }

    private async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        throw await _apiErrorParser.ParseApiErrorAsync(response, cancellationToken).ConfigureAwait(false);
    }

    private string BuildPath(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(_options.BaseUrl))
        {
            throw new InvalidOperationException(
                $"Outbound HTTP calls require '{BaseHttpClientOptions.SectionName}:BaseUrl' in configuration. "
                + "Set a non-empty absolute URL so the HttpClient BaseAddress is configured."
            );
        }

        var prefix = NormalizeSegment(_options.ApiPrefix);
        var path = NormalizeSegment(relativePath);
        return string.IsNullOrEmpty(prefix)
            ? path
            : string.IsNullOrEmpty(path)
                ? prefix
                : $"{prefix}/{path}";
    }

    private static string NormalizeSegment(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return value.Trim().Trim('/');
    }
}
