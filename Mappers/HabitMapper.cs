using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.DTOs.Responses;
using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.Mappers;

public static class HabitMapper
{
    public static Habit ToEntity(HabitRequestDto dto)
    {
        return new Habit
        {
            Discipline = new HabitDiscipline { Id = dto.DisciplineId },
            User = new PlayerUser { Id = dto.UserId },
            Title = dto.Title,
            Description = dto.Description,
        };
    }

    public static HabitResponseDto ToResponse(Habit habit)
    {
        return new HabitResponseDto
        {
            Id = habit.Id,
            Title = habit.Title,
            Description = habit.Description,
            DisciplineId = habit.Discipline.Id,
            UserId = habit.User.Id,
            IsActive = habit.IsActive,
        };
    }
}
