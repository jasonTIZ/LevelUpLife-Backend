namespace LevelUpLifeBackend.Models;

public class HabitTask
{
    public int Id { get; set; }
    public int HabitId { get; set; }
    public Habit? Habit { get; set; }
    public RepetitionCriteria? RepetitionCriteria { get; set; }

    public HabitTask()
    {
        Id = 0;
        HabitId = 0;
    }
}