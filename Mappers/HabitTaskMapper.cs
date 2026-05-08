using LevelUpLifeBackend.DTOs.Responses;
using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.Mappers;

public static class HabitTaskMapper
{
    public static HabitTaskResponseDto ToResponse(HabitTask task)
    {
        return new HabitTaskResponseDto
        {
            Id = task.Id,
            HabitId = task.HabitId,
            HabitDisciplineId = task.HabitDisciplineId,
            Title = task.Title,
            Description = task.Description,
            XpValue = task.XpValue,
            PeriodLength = task.PeriodLength,
            PeriodUnit = task.PeriodUnit,
            StartDate = task.StartDate,
            IsActive = task.IsActive,
            WeekDays = task.WeekDays,
            Difficulty = task.Difficulty,
            Frequency = task.Frequency,
            CompletionCriteria = task.CompletionCriteria,
            Evidence = task.Evidence,
            RepetitionCriteria = task.RepetitionCriteria is null
                ? null
                : RepetitionCriteriaMapper.ToResponse(task.RepetitionCriteria),
        };
    }
}
