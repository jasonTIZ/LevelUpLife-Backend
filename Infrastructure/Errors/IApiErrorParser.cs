namespace LevelUpLifeBackend.Infrastructure.Errors;

/// <summary>
/// Convierte una <see cref="HttpResponseMessage"/> fallida en un <see cref="AppError"/> tipado (issue #46).
/// </summary>
public interface IApiErrorParser
{
    /// <summary>Equivalente semántico a parseApiError(response) de la especificación.</summary>
    Task<AppError> ParseApiErrorAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken = default
    );
}
