namespace LevelUpLifeBackend.DTOs.Responses;

public class TimerCriteriaResponseDto
{
    public int Id { get; set; }
    public int HabitTaskId { get; set; }
    public int NumSecondsDefined { get; set; }
    public int? NumSecondsLong { get; set; }
    public bool TypePauseIsAllowed { get; set; }
    public bool StatusTimerCriteriaIsActive { get; set; }
}
