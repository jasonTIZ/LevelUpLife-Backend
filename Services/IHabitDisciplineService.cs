using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.DTOs.Responses;

namespace LevelUpLifeBackend.Services;

public interface IHabitDisciplineService
{
    Task<IEnumerable<HabitDisciplineDetailResponseDto>> GetAllDisciplinesAsync();

    Task<HabitDisciplineDetailResponseDto> GetDisciplineByIdAsync(int id);

    Task<HabitDisciplineDetailResponseDto> CreateDisciplineAsync(CreateHabitDisciplineRequestDto request);

    Task<(bool Success, string Message)> UpdateDisciplineStatusAsync(
        int id,
        UpdateHabitDisciplineStatusRequestDto request);
}
