using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.Repositories;

public interface ITimerCriteriaRepository
{
    Task<TimerCriteria?> GetByTaskIdAsync(int taskId);
}
