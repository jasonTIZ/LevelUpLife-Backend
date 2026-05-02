using LevelUpLifeBackend.Infrastructure.Http.Context;
using LevelUpLifeBackend.Infrastructure.Http.Events;
using Microsoft.AspNetCore.Http;

namespace LevelUpLifeBackend.Infrastructure.Http.Handlers;

public sealed class GlobalErrorHandler : DelegatingHandler
{
    private readonly ISessionResetService _sessionResetService;
    private readonly IGlobalErrorPublisher _globalErrorPublisher;

    public GlobalErrorHandler(
        ISessionResetService sessionResetService,
        IGlobalErrorPublisher globalErrorPublisher
    )
    {
        _sessionResetService = sessionResetService;
        _globalErrorPublisher = globalErrorPublisher;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        var response = await base.SendAsync(request, cancellationToken);

        if ((int)response.StatusCode == StatusCodes.Status401Unauthorized)
        {
            await _sessionResetService.ResetAsync(cancellationToken);
        }
        else if ((int)response.StatusCode >= StatusCodes.Status500InternalServerError)
        {
            _globalErrorPublisher.Publish(
                new ServerErrorEvent(
                    (int)response.StatusCode,
                    response.ReasonPhrase ?? "Server Error",
                    request.RequestUri?.AbsolutePath
                )
            );
        }

        return response;
    }
}
