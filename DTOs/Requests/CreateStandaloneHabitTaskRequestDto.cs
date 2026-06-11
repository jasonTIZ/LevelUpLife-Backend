using System.ComponentModel.DataAnnotations;
using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.DTOs.Requests;

public class CreateStandaloneHabitTaskRequestDto : IValidatableObject
{
    [Required(ErrorMessage = "HabitId is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "HabitId must be greater than 0.")]
    public int HabitId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "HabitDisciplineId must be greater than 0.")]
    public int? HabitDisciplineId { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 100 characters.")]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "XpValue cannot be negative.")]
    public int? XpValue { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "PeriodLength must be greater than 0.")]
    public int? PeriodLength { get; set; }

    public int? LongValue { get; set; }
    public DateOnly? StartDate { get; set; }
    public bool? IsActive { get; set; }
    public string? WeekDays { get; set; }

    [Required(ErrorMessage = "Difficulty is required.")]
    public TaskDifficulty? Difficulty { get; set; }

    [Required(ErrorMessage = "Frequency is required.")]
    public TaskFrequency? Frequency { get; set; }

    public TaskPeriodUnit? PeriodUnit { get; set; }

    [Required(ErrorMessage = "CompletionCriteria is required.")]
    public TaskCompletionCriteria? CompletionCriteria { get; set; }

    public TaskEvidence? Evidence { get; set; }

    public CreateRepetitionCriteriaRequestDto? RepetitionCriteria { get; set; }
    public CreateTimerCriteriaRequestDto? TimerCriteria { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (CompletionCriteria == TaskCompletionCriteria.REPETITIONS && RepetitionCriteria is null)
        {
            yield return new ValidationResult(
                "RepetitionCriteria is required when CompletionCriteria is REPETITIONS.",
                [nameof(RepetitionCriteria)]
            );
        }

        if (CompletionCriteria != TaskCompletionCriteria.REPETITIONS && RepetitionCriteria is not null)
        {
            yield return new ValidationResult(
                "RepetitionCriteria must be omitted unless CompletionCriteria is REPETITIONS.",
                [nameof(RepetitionCriteria)]
            );
        }

        if (CompletionCriteria == TaskCompletionCriteria.TIMER && TimerCriteria is null)
        {
            yield return new ValidationResult(
                "TimerCriteria is required when CompletionCriteria is TIMER.",
                [nameof(TimerCriteria)]
            );
        }

        if (CompletionCriteria != TaskCompletionCriteria.TIMER && TimerCriteria is not null)
        {
            yield return new ValidationResult(
                "TimerCriteria must be omitted unless CompletionCriteria is TIMER.",
                [nameof(TimerCriteria)]
            );
        }

        if (CompletionCriteria != TaskCompletionCriteria.EVIDENCE && Evidence is not null)
        {
            yield return new ValidationResult(
                "Evidence must be omitted unless CompletionCriteria is EVIDENCE.",
                [nameof(Evidence)]
            );
        }
    }
}
