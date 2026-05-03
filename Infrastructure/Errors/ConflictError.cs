namespace LevelUpLifeBackend.Infrastructure.Errors;

public sealed class ConflictError : AppError
{
    public ConflictError(ErrorResponse payload)
        : base(409, payload)
    {
    }
}
