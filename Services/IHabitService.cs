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
    Task<HabitResponseDto?> GetByIdAsync(int id);
    Task<PagedResultDto<HabitResponseDto>> GetActiveHabitsPaginatedAsync(
        int pageNumber,
        int pageSize,
        int userId
    );
    Task<HabitResponseDto?> UpdateHabitAsync(UpdateHabitRequestDto dto);
}
