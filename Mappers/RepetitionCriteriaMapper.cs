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
            NumRepetitionsObjective = dto.NumRepetitionsObjective ?? 0,
            TypeUnityMeasurementUnit = dto.TypeUnityMeasurementUnit ?? MeasurementUnit.REPS,
            StatusIsPartialAllowed = dto.StatusIsPartialAllowed ?? false,
            StatusRepetitionsCriteriaIsActive = dto.StatusRepetitionsCriteriaIsActive ?? true,
        };
    }

    public static RepetitionCriteriaResponseDto ToResponse(RepetitionCriteria repetitionCriteria)
    {
        return new RepetitionCriteriaResponseDto
        {
            Id = repetitionCriteria.Id,
            HabitTaskId = repetitionCriteria.HabitTaskId,
            NumRepetitionsObjective = repetitionCriteria.NumRepetitionsObjective,
            TypeUnityMeasurementUnit = repetitionCriteria.TypeUnityMeasurementUnit,
            StatusIsPartialAllowed = repetitionCriteria.StatusIsPartialAllowed,
            StatusRepetitionsCriteriaIsActive = repetitionCriteria.StatusRepetitionsCriteriaIsActive,
        };
    }
}