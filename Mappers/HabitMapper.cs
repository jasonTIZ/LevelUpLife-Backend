using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.DTOs.Responses;
using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.Mappers;

public static class HabitMapper
{
    public static Habit ToEntity(CreateHabitRequestDto dto)
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
            DisciplineName = habit.Discipline.Name,
            UserName = habit.User.UserName,
            CategoryName = habit.Discipline.Category.Name,
            IsActive = habit.IsActive,
            Tasks = habit.Tasks.Select(HabitTaskMapper.ToResponse).ToList(),
        };
    }

    public static void UpdateEntity(UpdateHabitRequestDto dto, Habit existingHabit)
    {
        existingHabit.Discipline.Id = dto.DisciplineId;
        existingHabit.Title = dto.Title;
        existingHabit.Description = dto.Description;
    }
}
