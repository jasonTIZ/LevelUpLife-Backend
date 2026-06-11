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
