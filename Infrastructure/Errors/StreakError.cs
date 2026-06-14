namespace LevelUpLifeBackend.Infrastructure.Errors;

public enum StreakFailureKind
{
    ProtectionLimitExceeded,
    ProtectionAlreadyActive,
    InvalidProtectionType,
}

public sealed class StreakError : AppError
{
    public StreakFailureKind Kind { get; }

    public StreakError(ErrorResponse payload, StreakFailureKind kind)
        : base(payload.Code, payload, payload.Message)
    {
        Kind = kind;
    }
}
