using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.DTOs.Responses;

public class HabitTaskResponseDto
{
    public int Id { get; set; }
    public int HabitId { get; set; }
    public HabitDisciplineDetailResponseDto? HabitDiscipline { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int XpValue { get; set; }
    public int PeriodLength { get; set; }
    public TaskPeriodUnit PeriodUnit { get; set; }
    public DateOnly StartDate { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsActive { get; set; }
    public string? WeekDays { get; set; }
    public TaskDifficulty Difficulty { get; set; }
    public TaskFrequency Frequency { get; set; }
    public TaskCompletionCriteria CompletionCriteria { get; set; }
    public TaskEvidence? Evidence { get; set; }
    public RepetitionCriteriaResponseDto? RepetitionCriteria { get; set; }
}
