using System.Net;
using System.Text;
using LevelUpLifeBackend.Infrastructure.Errors;
using LevelUpLifeBackend.Infrastructure.Http;
using Microsoft.Extensions.Options;
using Xunit;

namespace LevelUpLifeBackend.Tests;

public sealed class BaseApiClientIntegrationTests
{
    private sealed class StubHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _send;

        public StubHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> send)
        {
            _send = send;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken
        ) => _send(request, cancellationToken);
    }

    [Fact]
    public async Task GetAsync_on_404_throws_NotFoundError_via_parser()
    {
        var handler = new StubHandler(
            (_, _) =>
                Task.FromResult(
                    new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent("{}", Encoding.UTF8, "application/json"),
                    }
                )
        );

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://example.test/") };
        var options = Options.Create(
            new BaseHttpClientOptions
            {
                BaseUrl = "https://example.test/",
                ApiPrefix = string.Empty,
            }
        );
        var sut = new BaseApiClient(httpClient, options, new ApiErrorParser());

        await Assert.ThrowsAsync<NotFoundError>(() => sut.GetAsync("resource"));
    }
}
