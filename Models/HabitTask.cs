namespace LevelUpLifeBackend.Models;

public class HabitTask
{
    public int Id { get; set; }
    public int HabitId { get; set; }
    public Habit? Habit { get; set; }
    public int? HabitDisciplineId { get; set; }
    public HabitDiscipline? HabitDiscipline { get; set; }
    public RepetitionCriteria? RepetitionCriteria { get; set; }

    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? WeekDays { get; set; }
    public TaskDifficulty Difficulty { get; set; }
    public int XpValue { get; set; }
    public TaskFrequency Frequency { get; set; }
    public int PeriodLength { get; set; }
    public TaskPeriodUnit PeriodUnit { get; set; }
    public DateOnly StartDate { get; set; }
    public bool IsCompleted { get; set; }
    public TaskCompletionCriteria CompletionCriteria { get; set; }
    public TaskEvidence? Evidence { get; set; }
    public bool IsActive { get; set; }

    public HabitTask()
    {
        Id = 0;
        HabitId = 0;
        StartDate = DateOnly.FromDateTime(DateTime.UtcNow);
        CompletionCriteria = TaskCompletionCriteria.REPETITIONS;
        Difficulty = TaskDifficulty.MEDIUM;
        Frequency = TaskFrequency.DAILY;
        XpValue = 0;
        PeriodLength = 1;
        PeriodUnit = TaskPeriodUnit.DAYS;
        IsCompleted = false;
        IsActive = true;
    }
}
