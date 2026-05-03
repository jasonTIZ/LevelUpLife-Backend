namespace LevelUpLifeBackend.Infrastructure.Errors;

public sealed class NotFoundError : AppError
{
    public NotFoundError(ErrorResponse payload)
        : base(404, payload)
    {
    }
}
