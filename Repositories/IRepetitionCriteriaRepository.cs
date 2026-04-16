using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.Repositories;

public interface IRepetitionCriteriaRepository
{
    Task<bool> HabitTaskExistsAsync(int taskId);
    Task<bool> CriteriaExistsAsync(int taskId);
    Task<RepetitionCriteria> AddAsync(RepetitionCriteria criteria);
}