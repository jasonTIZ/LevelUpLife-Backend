using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.Repositories;

public interface IRepetitionCriteriaRepository
{
    Task<RepetitionCriteria> AddAsync(RepetitionCriteria criteria);
}