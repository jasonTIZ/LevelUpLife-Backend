using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.DTOs.Responses;
using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.Mappers;

public static class RepetitionCriteriaMapper
{
    public static RepetitionCriteria ToEntity(int taskId, CreateRepetitionCriteriaRequestDto dto)
    {
        return new RepetitionCriteria
        {
            HabitTaskId = taskId,
            NumRepetitionsObjective = dto.Repetitions ?? 0,
            TypeUnityMeasurementUnit = dto.MeasurementUnit ?? Models.MeasurementUnit.REPS,
            StatusIsPartialAllowed = dto.IsPartialAllowed ?? false,
            StatusRepetitionsCriteriaIsActive = dto.IsActive ?? true,
        };
    }

    public static void UpdateEntity(RepetitionCriteria entity, UpdateRepetitionCriteriaRequestDto dto)
    {
        entity.NumRepetitionsObjective = dto.Repetitions;
        entity.TypeUnityMeasurementUnit = dto.MeasurementUnit;
        if (dto.IsPartialAllowed.HasValue)
            entity.StatusIsPartialAllowed = dto.IsPartialAllowed.Value;
        if (dto.IsActive.HasValue)
            entity.StatusRepetitionsCriteriaIsActive = dto.IsActive.Value;
    }

    public static RepetitionCriteriaResponseDto ToResponse(RepetitionCriteria entity)
    {
        return new RepetitionCriteriaResponseDto
        {
            Id = entity.Id,
            HabitTaskId = entity.HabitTaskId,
            Repetitions = entity.NumRepetitionsObjective,
            MeasurementUnit = entity.TypeUnityMeasurementUnit,
            IsPartialAllowed = entity.StatusIsPartialAllowed,
            IsActive = entity.StatusRepetitionsCriteriaIsActive,
        };
    }
}
