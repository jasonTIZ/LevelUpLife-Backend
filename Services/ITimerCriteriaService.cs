namespace LevelUpLifeBackend.Services;

public interface ITimerCriteriaService
{
    Task DeactivateAsync(int taskId, int id);
}