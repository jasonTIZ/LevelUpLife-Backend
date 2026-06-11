using LevelUpLifeBackend.DTOs.Responses;

namespace LevelUpLifeBackend.Services;

public interface IHabitTaskService
{
    Task<HabitTaskResponseDto> GetByIdAsync(int taskId);
    Task<IEnumerable<EvidenceStorageResponseDto>> GetEvidencesByTaskIdAsync(int taskId);
    Task<EvidenceStorageResponseDto> GetEvidenceByIdAsync(int taskId, int id);
}
