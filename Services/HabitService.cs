using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.DTOs.Responses;
using LevelUpLifeBackend.Mappers;
using LevelUpLifeBackend.Repositories;

namespace LevelUpLifeBackend.Services;

public class HabitService : IHabitService
{
    private readonly IHabitRepository _habitRepository;

    public HabitService(IHabitRepository habitRepository)
    {
        _habitRepository = habitRepository;
    }

    public async Task<HabitResponseDto> CreateAsync(CreateHabitRequestDto request)
    {
        return HabitMapper.ToResponse(
            await _habitRepository.AddAsync(HabitMapper.ToEntity(request))
        );
    }

    public async Task<HabitResponseDto?> GetByIdAsync(int id)
    {
        var habit = await _habitRepository.GetByIdAsync(id);
        return habit is null ? null : HabitMapper.ToResponse(habit);
    }
}
