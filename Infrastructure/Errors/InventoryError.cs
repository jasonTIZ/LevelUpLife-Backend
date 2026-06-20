namespace LevelUpLifeBackend.Infrastructure.Errors;

public enum InventoryFailureKind
{
    InsufficientGold,
    ItemNotActivatable,
    NoRecoveryTarget,
    EffectAlreadyActive,
}

public sealed class InventoryError : AppError
{
    public InventoryFailureKind Kind { get; }

    public InventoryError(ErrorResponse payload, InventoryFailureKind kind)
        : base(payload.Code, payload, payload.Message)
    {
        Kind = kind;
    }
}
