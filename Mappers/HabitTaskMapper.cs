using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.DTOs.Responses;
using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.Mappers;

public static class HabitTaskMapper
{
    public static void ApplyStandaloneRequest(
        CreateStandaloneHabitTaskRequestDto request,
        HabitTask task,
        int habitDisciplineId
    )
    {
        task.HabitDisciplineId = request.HabitDisciplineId ?? habitDisciplineId;
        task.Title = request.Title.Trim();
        task.Description = request.Description;
        task.WeekDays = request.WeekDays;
        task.Difficulty = request.Difficulty ?? task.Difficulty;
        task.XpValue = request.XpValue ?? 0;
        task.Frequency = request.Frequency!.Value;
        task.PeriodLength = request.PeriodLength ?? 1;
        task.PeriodUnit = request.PeriodUnit ?? TaskPeriodUnit.DAYS;
        task.StartDate = request.StartDate ?? task.StartDate;
        task.CompletionCriteria = request.CompletionCriteria!.Value;
        task.Evidence = request.Evidence;
        task.IsActive = request.IsActive ?? task.IsActive;
    }

    public static void ApplyNestedUpdateRequest(
        UpdateHabitTaskRequestDto request,
        HabitTask task,
        int habitDisciplineId
    )
    {
        if (request.HabitDisciplineId.HasValue)
            task.HabitDisciplineId = request.HabitDisciplineId;
        else if (request.Title is not null || request.Difficulty.HasValue)
            task.HabitDisciplineId ??= habitDisciplineId;

        if (request.Title is not null)
            task.Title = request.Title.Trim();
        if (request.Description is not null)
            task.Description = request.Description;
        if (request.WeekDays is not null)
            task.WeekDays = request.WeekDays;
        if (request.Difficulty.HasValue)
            task.Difficulty = request.Difficulty.Value;
        if (request.XpValue.HasValue)
            task.XpValue = request.XpValue.Value;
        if (request.Frequency.HasValue)
            task.Frequency = request.Frequency.Value;
        if (request.PeriodLength.HasValue)
            task.PeriodLength = request.PeriodLength.Value;
        if (request.PeriodUnit.HasValue)
            task.PeriodUnit = request.PeriodUnit.Value;
        if (request.StartDate.HasValue)
            task.StartDate = request.StartDate.Value;
        if (request.CompletionCriteria.HasValue)
            task.CompletionCriteria = request.CompletionCriteria.Value;
        if (request.Evidence.HasValue)
            task.Evidence = request.Evidence;
        if (request.IsActive.HasValue)
            task.IsActive = request.IsActive.Value;
    }

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
            Difficulty = dto.Difficulty ?? TaskDifficulty.MEDIUM,
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
            Difficulty = dto.Difficulty ?? TaskDifficulty.MEDIUM,
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
            HabitDiscipline = task.HabitDiscipline is null ? null : new HabitDisciplineDetailResponseDto
            {
                IdHabitDiscipline = task.HabitDiscipline.Id,
                IdHabitCategory = task.HabitDiscipline.Category.Id,
                DscHabitDisciplineName = task.HabitDiscipline.Name,
                DscHabitDisciplineDescription = task.HabitDiscipline.Description,
                StatusHabitDisciplineIsActive = task.HabitDiscipline.IsActive,
            },
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
