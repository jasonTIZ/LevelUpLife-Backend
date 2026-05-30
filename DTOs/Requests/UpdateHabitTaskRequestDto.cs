using System.ComponentModel.DataAnnotations;
using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.DTOs.Requests;

public class UpdateHabitTaskRequestDto : IValidatableObject
{
    [Required(ErrorMessage = "El identificador de la tarea es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "La tarea no es válida.")]
    public int TaskId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "HabitDisciplineId must be greater than 0.")]
    public int? HabitDisciplineId { get; set; }

    [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 100 characters.")]
    public string? Title { get; set; }

    public string? Description { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "XpValue cannot be negative.")]
    public int? XpValue { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "PeriodLength must be greater than 0.")]
    public int? PeriodLength { get; set; }

    public DateOnly? StartDate { get; set; }
    public bool? IsActive { get; set; }
    public string? WeekDays { get; set; }
    public TaskDifficulty? Difficulty { get; set; }
    public TaskFrequency? Frequency { get; set; }
    public TaskPeriodUnit? PeriodUnit { get; set; }
    public TaskCompletionCriteria? CompletionCriteria { get; set; }
    public TaskEvidence? Evidence { get; set; }

    public UpdateRepetitionCriteriaRequestDto? RepetitionCriteria { get; set; }

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

        if (CompletionCriteria != TaskCompletionCriteria.EVIDENCE && Evidence is not null)
        {
            yield return new ValidationResult(
                "Evidence must be omitted unless CompletionCriteria is EVIDENCE.",
                [nameof(Evidence)]
            );
        }
    }
}
