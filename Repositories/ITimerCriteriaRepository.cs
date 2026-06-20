using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.Repositories;

public interface ITimerCriteriaRepository
{
    Task<TimerCriteria> AddAsync(TimerCriteria criteria);
    Task<TimerCriteria?> GetByTaskIdAsync(int taskId);
    Task<TimerCriteria> UpdateAsync(TimerCriteria criteria);
}
