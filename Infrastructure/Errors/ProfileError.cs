namespace LevelUpLifeBackend.Infrastructure.Errors;

public enum ProfileFailureKind
{
    ETagMismatch,
}

/// <summary>412 Precondition Failed u otros errores de perfil/concurrencia.</summary>
public sealed class ProfileError : AppError
{
    public ProfileFailureKind Kind { get; }

    public ProfileError(ErrorResponse payload, ProfileFailureKind kind = ProfileFailureKind.ETagMismatch)
        : base(412, payload, kind == ProfileFailureKind.ETagMismatch ? $"ETAG_MISMATCH: {payload.Message}" : payload.Message)
    {
        Kind = kind;
    }
}
