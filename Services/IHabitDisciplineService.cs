using LevelUpLifeBackend.DTOs.Responses;

namespace LevelUpLifeBackend.Services;

public interface IHabitDisciplineService
{
    Task<HabitDisciplineDetailResponseDto> GetDisciplineByIdAsync(int id);
}
