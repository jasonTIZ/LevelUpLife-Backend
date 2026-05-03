using System.Text.Json;

namespace LevelUpLifeBackend.Infrastructure.Errors;

public sealed class ApiErrorParser : IApiErrorParser
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<AppError> ParseApiErrorAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken = default
    )
    {
        var status = (int)response.StatusCode;
        var rawBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        var payload = NormalizePayload(status, response.ReasonPhrase, rawBody);
        return MapToError(status, payload);
    }

    internal static ErrorResponse NormalizePayload(int httpStatus, string? reasonPhrase, string rawBody)
    {
        ErrorResponse? parsed = null;
        if (!string.IsNullOrWhiteSpace(rawBody))
        {
            try
            {
                parsed = JsonSerializer.Deserialize<ErrorResponse>(rawBody, JsonOptions);
            }
            catch (JsonException)
            {
                // JSON inválido: se usa fallback abajo.
            }
        }

        if (parsed is null)
        {
            return new ErrorResponse
            {
                Code = httpStatus,
                Message = string.IsNullOrWhiteSpace(reasonPhrase)
                    ? "Unexpected error"
                    : reasonPhrase.Trim(),
                Details = TruncateDetails(rawBody),
            };
        }

        if (parsed.Code == 0)
        {
            parsed.Code = httpStatus;
        }

        if (string.IsNullOrWhiteSpace(parsed.Message))
        {
            parsed.Message = string.IsNullOrWhiteSpace(reasonPhrase)
                ? "Unexpected error"
                : reasonPhrase.Trim();
        }

        parsed.Details ??= string.Empty;
        return parsed;
    }

    private static AppError MapToError(int status, ErrorResponse payload)
    {
        return status switch
        {
            401 => new AuthError(401, payload, AuthFailureKind.Unauthorized),
            403 => new AuthError(403, payload, AuthFailureKind.Forbidden),
            404 => new NotFoundError(payload),
            409 => new ConflictError(payload),
            412 => new ProfileError(payload, ProfileFailureKind.ETagMismatch),
            >= 500 and < 600 => new ServerError(status, payload),
            _ => new UnexpectedApiError(status, payload),
        };
    }

    private static string TruncateDetails(string raw, int max = 8000)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return string.Empty;
        }

        raw = raw.Trim();
        return raw.Length <= max ? raw : raw[..max];
    }
}
