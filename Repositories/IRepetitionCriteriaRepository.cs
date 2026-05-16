using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.Repositories;

public interface IRepetitionCriteriaRepository
{
    Task<RepetitionCriteria> AddAsync(RepetitionCriteria criteria);
    Task<RepetitionCriteria?> GetByTaskIdAsync(int taskId);
    Task<RepetitionCriteria> UpdateAsync(RepetitionCriteria criteria);
}
