using System.Net;
using System.Text;
using System.Text.Json;
using LevelUpLifeBackend.Infrastructure.Errors;
using Xunit;

namespace LevelUpLifeBackend.Tests;

public sealed class ApiErrorParserTests
{
    private readonly ApiErrorParser _parser = new();

    [Fact]
    public async Task ParseApiErrorAsync_maps_error_body_code_message_details()
    {
        var body = JsonSerializer.Serialize(
            new ErrorResponse
            {
                Code = 422,
                Message = "Validation failed",
                Details = "Name is required",
            }
        );

        var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent(body, Encoding.UTF8, "application/json"),
        };

        var error = await _parser.ParseApiErrorAsync(response);

        var unexpected = Assert.IsType<UnexpectedApiError>(error);
        Assert.Equal(400, unexpected.HttpStatusCode);
        Assert.Equal(422, unexpected.Payload.Code);
        Assert.Equal("Validation failed", unexpected.Payload.Message);
        Assert.Equal("Name is required", unexpected.Payload.Details);
    }

    [Fact]
    public async Task ParseApiErrorAsync_404_returns_NotFoundError()
    {
        var response = new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent("{}", Encoding.UTF8, "application/json"),
        };

        var error = await _parser.ParseApiErrorAsync(response);
        Assert.IsType<NotFoundError>(error);
    }

    [Fact]
    public async Task ParseApiErrorAsync_409_returns_ConflictError()
    {
        var response = new HttpResponseMessage(HttpStatusCode.Conflict)
        {
            Content = new StringContent("{}", Encoding.UTF8, "application/json"),
        };

        var error = await _parser.ParseApiErrorAsync(response);
        Assert.IsType<ConflictError>(error);
    }

    [Fact]
    public async Task ParseApiErrorAsync_412_returns_ProfileError_with_ETAG_MISMATCH()
    {
        var response = new HttpResponseMessage(HttpStatusCode.PreconditionFailed)
        {
            Content = new StringContent("{}", Encoding.UTF8, "application/json"),
        };

        var error = await _parser.ParseApiErrorAsync(response);
        var profile = Assert.IsType<ProfileError>(error);
        Assert.Equal(ProfileFailureKind.ETagMismatch, profile.Kind);
        Assert.Contains("ETAG_MISMATCH", profile.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ParseApiErrorAsync_500_returns_ServerError()
    {
        var response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("{}", Encoding.UTF8, "application/json"),
        };

        var error = await _parser.ParseApiErrorAsync(response);
        Assert.IsType<ServerError>(error);
    }

    [Fact]
    public async Task ParseApiErrorAsync_401_returns_AuthError_unauthorized()
    {
        var response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
        {
            Content = new StringContent("{}", Encoding.UTF8, "application/json"),
        };

        var error = await _parser.ParseApiErrorAsync(response);
        var auth = Assert.IsType<AuthError>(error);
        Assert.Equal(AuthFailureKind.Unauthorized, auth.Kind);
    }

    [Fact]
    public async Task ParseApiErrorAsync_403_returns_AuthError_forbidden()
    {
        var response = new HttpResponseMessage(HttpStatusCode.Forbidden)
        {
            Content = new StringContent("{}", Encoding.UTF8, "application/json"),
        };

        var error = await _parser.ParseApiErrorAsync(response);
        var auth = Assert.IsType<AuthError>(error);
        Assert.Equal(AuthFailureKind.Forbidden, auth.Kind);
        Assert.Contains("FORBIDDEN", auth.Message, StringComparison.Ordinal);
    }
}
