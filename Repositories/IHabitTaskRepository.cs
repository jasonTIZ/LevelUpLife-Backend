using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.Repositories;

public interface IHabitTaskRepository
{
    Task<HabitTask> AddAsync(HabitTask task);
    Task<bool> ExistsActiveByHabitIdAsync(int habitId);
    Task<HabitTask?> GetByIdWithCriteriaAsync(int id);
}
