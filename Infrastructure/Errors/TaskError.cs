namespace LevelUpLifeBackend.Infrastructure.Errors;

public enum TaskFailureKind
{
    CompletionRequirementsNotMet,
}

/// <summary>400 Bad Request for habit task business rule violations.</summary>
public sealed class TaskError : AppError
{
    public TaskFailureKind Kind { get; }

    public TaskError(ErrorResponse payload, TaskFailureKind kind = TaskFailureKind.CompletionRequirementsNotMet)
        : base(400, payload, payload.Message)
    {
        Kind = kind;
    }
}
