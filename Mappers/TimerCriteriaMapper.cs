using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.DTOs.Responses;
using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.Mappers;

public static class TimerCriteriaMapper
{
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
