using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.DTOs.Responses;

namespace LevelUpLifeBackend.Services;

public interface IHabitService
{
    Task<HabitResponseDto> CreateAsync(CreateHabitRequestDto request);
    Task<HabitTaskResponseDto> CreateTaskAsync(CreateStandaloneHabitTaskRequestDto request, int userId);
    Task<HabitTaskResponseDto> UpdateTaskAsync(
        int taskId,
        CreateStandaloneHabitTaskRequestDto request,
        int userId
    );
    Task DeactivateTaskAsync(int taskId, int userId);
    Task<CompleteHabitTaskResponseDto> CompleteTaskAsync(
        int taskId,
        int userId,
        CompleteHabitTaskRequestDto request
    );
    Task<HabitResponseDto?> GetByIdAsync(int id, int userId);
    Task<PagedResultDto<HabitResponseDto>> GetActiveHabitsPaginatedAsync(
        int pageNumber,
        int pageSize,
        int userId
    );
    Task<HabitResponseDto?> UpdateHabitAsync(UpdateHabitRequestDto dto);
}
