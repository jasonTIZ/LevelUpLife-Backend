namespace LevelUpLifeBackend.Infrastructure.Errors;

/// <summary>5xx del servidor remoto.</summary>
public sealed class ServerError : AppError
{
    public ServerError(int httpStatusCode, ErrorResponse payload)
        : base(httpStatusCode, payload)
    {
    }
}
