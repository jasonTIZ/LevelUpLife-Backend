using LevelUpLifeBackend.DTOs.Responses;
using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.Mappers;

public static class TimerCriteriaMapper
{
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
