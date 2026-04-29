namespace LevelUpLifeBackend.Exceptions;

public class RepetitionCriteriaAlreadyExistsException : Exception
{
    public const string ErrorCode = "CRITERIA_ALREADY_EXISTS";

    public RepetitionCriteriaAlreadyExistsException()
        : base("Ya existe un criterio de repetición para esta tarea.")
    {
    }
}