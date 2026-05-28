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
        task.Difficulty = request.Difficulty!.Value;
        task.XpValue = request.XpValue ?? 0;
        task.Frequency = request.Frequency!.Value;
        task.PeriodLength = request.PeriodLength ?? 1;
        task.PeriodUnit = request.PeriodUnit ?? TaskPeriodUnit.DAYS;
        task.StartDate = request.StartDate ?? task.StartDate;
        task.CompletionCriteria = request.CompletionCriteria!.Value;
        task.Evidence = request.Evidence;
        task.IsActive = request.IsActive ?? task.IsActive;
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
