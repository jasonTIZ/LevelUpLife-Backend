using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.DTOs.Responses;

namespace LevelUpLifeBackend.Services;

public interface ITimerCriteriaService
{
    Task<TimerCriteriaResponseDto> CreateAsync(int taskId, CreateTimerCriteriaRequestDto request);
}
