using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.DTOs.Responses;
using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.Mappers;

public static class HabitTaskMapper
{
    public static HabitTask ToEntity(
        CreateHabitTaskRequestDto dto,
        int habitId,
        int habitDisciplineId
    )
    {
        return new HabitTask
        {
            HabitId = habitId,
            HabitDisciplineId = dto.HabitDisciplineId ?? habitDisciplineId,
            Title = dto.Title,
            Description = dto.Description,
            WeekDays = dto.WeekDays,
            Difficulty = dto.Difficulty!.Value,
            XpValue = dto.XpValue ?? 0,
            Frequency = dto.Frequency!.Value,
            PeriodLength = dto.PeriodLength!.Value,
            PeriodUnit = dto.PeriodUnit!.Value,
            StartDate = dto.StartDate!.Value,
            CompletionCriteria = dto.CompletionCriteria!.Value,
            Evidence = dto.Evidence,
            IsActive = dto.IsActive ?? true,
            IsCompleted = false,
        };
    }

    public static HabitTask ToEntity(
        CreateStandaloneHabitTaskRequestDto dto,
        int habitDisciplineId
    )
    {
        return new HabitTask
        {
            HabitId = dto.HabitId,
            HabitDisciplineId = dto.HabitDisciplineId ?? habitDisciplineId,
            Title = dto.Title.Trim(),
            Description = dto.Description,
            WeekDays = dto.WeekDays,
            Difficulty = dto.Difficulty!.Value,
            XpValue = dto.XpValue ?? 0,
            Frequency = dto.Frequency!.Value,
            PeriodLength = dto.PeriodLength ?? 1,
            PeriodUnit = dto.PeriodUnit ?? TaskPeriodUnit.DAYS,
            StartDate = dto.StartDate ?? DateOnly.FromDateTime(DateTime.UtcNow),
            CompletionCriteria = dto.CompletionCriteria!.Value,
            Evidence = dto.Evidence,
            IsActive = dto.IsActive ?? true,
            IsCompleted = false,
        };
    }

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
            IsCompleted = task.IsCompleted,
            IsActive = task.IsActive,
            WeekDays = task.WeekDays,
            Difficulty = task.Difficulty,
            Frequency = task.Frequency,
            CompletionCriteria = task.CompletionCriteria,
            Evidence = task.Evidence,
            RepetitionCriteria = task.RepetitionCriteria is null
                ? null
                : RepetitionCriteriaMapper.ToResponse(task.RepetitionCriteria),
            TimerCriteria = task.TimerCriteria is null
                ? null
                : TimerCriteriaMapper.ToResponse(task.TimerCriteria),
        };
    }
}
