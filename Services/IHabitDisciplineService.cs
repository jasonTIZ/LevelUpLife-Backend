using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.DTOs.Responses;

namespace LevelUpLifeBackend.Services;

public interface IHabitDisciplineService
{
    Task<HabitDisciplineDetailResponseDto> GetDisciplineByIdAsync(int id);

    Task<HabitDisciplineDetailResponseDto> CreateDisciplineAsync(CreateHabitDisciplineRequestDto request);

    Task<HabitDisciplineDetailResponseDto> UpdateDisciplineAsync(
        int id,
        UpdateHabitDisciplineRequestDto request);
}
