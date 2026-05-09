namespace LevelUpLifeBackend.Infrastructure.Errors;

public enum AuthFailureKind
{
    Unauthorized,
    Forbidden,
}

/// <summary>401 o 403 mapeados a error de autenticación/autorización.</summary>
public sealed class AuthError : AppError
{
    public AuthFailureKind Kind { get; }

    public AuthError(int httpStatusCode, ErrorResponse payload, AuthFailureKind kind)
        : base(httpStatusCode, payload, BuildMessage(payload, kind))
    {
        Kind = kind;
    }

    private static string BuildMessage(ErrorResponse payload, AuthFailureKind kind)
    {
        return kind == AuthFailureKind.Forbidden
            ? $"FORBIDDEN: {payload.Message}"
            : payload.Message;
    }
}
