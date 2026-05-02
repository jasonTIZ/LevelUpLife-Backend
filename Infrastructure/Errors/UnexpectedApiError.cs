namespace LevelUpLifeBackend.Infrastructure.Errors;

/// <summary>Error HTTP no contemplado explícitamente en el mapa de la issue #46.</summary>
public sealed class UnexpectedApiError : AppError
{
    public UnexpectedApiError(int httpStatusCode, ErrorResponse payload)
        : base(httpStatusCode, payload)
    {
    }
}
