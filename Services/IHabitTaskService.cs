using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.DTOs.Responses;

namespace LevelUpLifeBackend.Services;

public interface IHabitTaskService
{
    Task<HabitTaskResponseDto> GetByIdAsync(int taskId);
    Task<EvidenceStorageResponseDto> CreateEvidenceAsync(int taskId, CreateEvidenceRequestDto request);
    Task<IEnumerable<EvidenceStorageResponseDto>> GetEvidencesByTaskIdAsync(int taskId);
    Task<EvidenceStorageResponseDto> GetEvidenceByIdAsync(int taskId, int id);
}
