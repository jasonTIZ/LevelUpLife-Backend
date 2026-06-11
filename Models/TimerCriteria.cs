namespace LevelUpLifeBackend.Models;

public class TimerCriteria
{
    public int Id { get; set; }
    public int HabitTaskId { get; set; }
    public HabitTask? HabitTask { get; set; }
    public int NumSecondsDefined { get; set; }
    public int? NumSecondsLong { get; set; }
    public bool TypePauseIsAllowed { get; set; }
    public bool StatusTimerCriteriaIsActive { get; set; }

    public TimerCriteria()
    {
        Id = 0;
        HabitTaskId = 0;
        NumSecondsDefined = 0;
        NumSecondsLong = null;
        TypePauseIsAllowed = false;
        StatusTimerCriteriaIsActive = true;
    }
}