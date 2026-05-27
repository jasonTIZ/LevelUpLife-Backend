using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.Repositories;

public interface IHabitTaskRepository
{
    Task<bool> HabitBelongsToUserAsync(int habitId, int userId);
    Task<(IReadOnlyList<HabitTask> Items, int Total)> ListByHabitAndUserAsync(
        int habitId,
        int userId,
        HabitTaskListQueryDto filter
    );
    Task<HabitTask> AddAsync(HabitTask task);
    Task<bool> ExistsActiveByHabitIdAsync(int habitId);
    Task<HabitTask?> GetByIdWithCriteriaAsync(int id);
    Task<IEnumerable<EvidenceStorage>> GetEvidencesByTaskIdAsync(int taskId);
    Task<EvidenceStorage?> GetEvidenceByIdAsync(int taskId, int id);
    Task<bool> ExistsAsync(int taskId);
}
