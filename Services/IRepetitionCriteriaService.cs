using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.DTOs.Responses;

namespace LevelUpLifeBackend.Services;

public interface IRepetitionCriteriaService
{
    Task<RepetitionCriteriaResponseDto?> CreateAsync(int taskId, CreateRepetitionCriteriaRequestDto request);
}