using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.DTOs.Responses;

namespace LevelUpLifeBackend.Services;

public interface IHabitService
{
    public Task<HabitResponseDto> CreateAsync(CreateHabitRequestDto request);
    public Task<HabitResponseDto?> GetByIdAsync(int id);
}
