namespace LevelUpLifeBackend.Infrastructure.Errors;

/// <summary>
/// Error de aplicación tipado, construido a partir de una respuesta HTTP y/o <see cref="ErrorResponse"/>.
/// </summary>
public class AppError : Exception
{
    public int HttpStatusCode { get; }
    public ErrorResponse Payload { get; }

    public AppError(int httpStatusCode, ErrorResponse payload, string? message = null)
        : base(message ?? payload.Message)
    {
        HttpStatusCode = httpStatusCode;
        Payload = payload;
    }
}
