using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.Repositories;

public interface IHabitTaskRepository
{
    Task<HabitTask> AddAsync(HabitTask task);
    Task<IEnumerable<EvidenceStorage>> GetEvidencesByTaskIdAsync(int taskId);
    Task<EvidenceStorage?> GetEvidenceByIdAsync(int taskId, int id);
    Task<bool> ExistsAsync(int taskId);
}
