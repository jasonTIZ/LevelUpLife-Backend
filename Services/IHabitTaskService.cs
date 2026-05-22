using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.DTOs.Responses;

namespace LevelUpLifeBackend.Services;

public interface IHabitTaskService
{
    Task<HabitTaskPagedResponseDto> ListAsync(HabitTaskListQueryDto query);
    Task<IEnumerable<EvidenceStorageResponseDto>> GetEvidencesByTaskIdAsync(int taskId);
    Task<EvidenceStorageResponseDto> GetEvidenceByIdAsync(int taskId, int id);
}
