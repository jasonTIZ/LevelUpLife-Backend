using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.Repositories;

public interface IHabitTaskRepository
{
    Task<HabitTask> AddAsync(HabitTask task);
    Task UpdateAsync(HabitTask task);
    Task<bool> ExistsActiveByHabitIdAsync(int habitId);
    Task<HabitTask?> GetByIdWithCriteriaAsync(int id);
    Task<HabitTask?> GetTrackedByIdForUserAsync(int taskId, int userId);
    Task<IEnumerable<EvidenceStorage>> GetEvidencesByTaskIdAsync(int taskId);
    Task<EvidenceStorage?> GetEvidenceByIdAsync(int taskId, int id);
    Task<bool> ExistsAsync(int taskId);
}
