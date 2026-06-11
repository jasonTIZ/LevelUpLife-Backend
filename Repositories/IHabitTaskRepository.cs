using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.Repositories;

public interface IHabitTaskRepository
{
    Task<HabitTask> AddAsync(HabitTask task);
    Task<HabitTask?> GetTrackedByIdForUserAsync(int taskId, int userId);
    Task UpdateWithRepetitionCriteriaAsync(HabitTask task);
    Task<HabitTask?> GetByIdWithCriteriaAsync(int id);
    Task<EvidenceStorage> AddEvidenceAsync(EvidenceStorage evidence);
    Task<IEnumerable<EvidenceStorage>> GetEvidencesByTaskIdAsync(int taskId);
    Task<EvidenceStorage?> GetEvidenceByIdAsync(int taskId, int id);
    Task<bool> ExistsAsync(int taskId);
}
