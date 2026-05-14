namespace LevelUpLifeBackend.Services;

public interface IRepetitionCriteriaService
{
    Task DeactivateAsync(int taskId, int id);
}
