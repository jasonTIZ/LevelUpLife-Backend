using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.DTOs.Responses;
using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.Mappers;

public static class TimerCriteriaMapper
{
    public static TimerCriteria ToEntity(int taskId, CreateTimerCriteriaRequestDto dto)
    {
        return new TimerCriteria
        {
            HabitTaskId = taskId,
            NumSecondsDefined = dto.NUM_SECONDS_DEFINED ?? 0,
            NumSecondsLong = dto.NUM_SECONDS_LONG,
            TypePauseIsAllowed = dto.TYPE_PAUSE_IS_ALLOWED ?? false,
            StatusTimerCriteriaIsActive = dto.STATUS_TIMER_CRITERIA_IS_ACTIVE ?? true,
        };
    }

    // Used when updating a task inline via CreateStandaloneHabitTaskRequestDto (POST/PUT /habit-tasks)
    public static void UpdateEntity(TimerCriteria entity, CreateTimerCriteriaRequestDto dto)
    {
        entity.NumSecondsDefined = dto.NUM_SECONDS_DEFINED ?? entity.NumSecondsDefined;
        entity.NumSecondsLong = dto.NUM_SECONDS_LONG;
        entity.TypePauseIsAllowed = dto.TYPE_PAUSE_IS_ALLOWED ?? entity.TypePauseIsAllowed;
        if (dto.STATUS_TIMER_CRITERIA_IS_ACTIVE.HasValue)
            entity.StatusTimerCriteriaIsActive = dto.STATUS_TIMER_CRITERIA_IS_ACTIVE.Value;
    }

    // Used when updating timer criteria nested within UpdateHabitRequestDto (PUT /api/Habits/{id})
    public static void UpdateEntity(TimerCriteria entity, UpdateTimerCriteriaRequestDto dto)
    {
        entity.NumSecondsDefined = dto.NumSecondsDefined;
        entity.NumSecondsLong = dto.NumSecondsLong;
        entity.TypePauseIsAllowed = dto.TypePauseIsAllowed;
        if (dto.IsActive.HasValue)
            entity.StatusTimerCriteriaIsActive = dto.IsActive.Value;
    }

    public static TimerCriteriaResponseDto ToResponse(TimerCriteria entity)
    {
        return new TimerCriteriaResponseDto
        {
            Id = entity.Id,
            HabitTaskId = entity.HabitTaskId,
            NumSecondsDefined = entity.NumSecondsDefined,
            NumSecondsLong = entity.NumSecondsLong,
            TypePauseIsAllowed = entity.TypePauseIsAllowed,
            StatusTimerCriteriaIsActive = entity.StatusTimerCriteriaIsActive,
        };
    }
}
