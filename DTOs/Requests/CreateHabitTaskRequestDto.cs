using System.ComponentModel.DataAnnotations;
using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.DTOs.Requests;

public class CreateHabitTaskRequestDto
    : IValidatableObject
{
    [Required(ErrorMessage = "El título de la tarea es obligatorio.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El título debe tener entre 3 y 100 caracteres.")]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La disciplina de la tarea no es válida.")]
    public int? HabitDisciplineId { get; set; }

    public string? WeekDays { get; set; }

    [Required(ErrorMessage = "La dificultad es obligatoria.")]
    public TaskDifficulty? Difficulty { get; set; }

    [Required(ErrorMessage = "La frecuencia es obligatoria.")]
    public TaskFrequency? Frequency { get; set; }

    [Required(ErrorMessage = "La duración del período es obligatoria.")]
    [Range(1, int.MaxValue, ErrorMessage = "El período debe ser mayor a 0.")]
    public int? PeriodLength { get; set; }

    [Required(ErrorMessage = "La unidad del período es obligatoria.")]
    public TaskPeriodUnit? PeriodUnit { get; set; }

    [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
    public DateOnly? StartDate { get; set; }

    [Required(ErrorMessage = "El criterio de completado es obligatorio.")]
    public TaskCompletionCriteria? CompletionCriteria { get; set; }

    public TaskEvidence? Evidence { get; set; }
    public CreateRepetitionCriteriaRequestDto? RepetitionCriteria { get; set; }
    public CreateTimerCriteriaRequestDto? TimerCriteria { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "El XP no puede ser negativo.")]
    public int? XpValue { get; set; }

    public bool? IsActive { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (CompletionCriteria == TaskCompletionCriteria.REPETITIONS && RepetitionCriteria is null)
        {
            yield return new ValidationResult(
                "Los criterios de repetición son obligatorios cuando el criterio de completado es REPETITIONS.",
                [nameof(RepetitionCriteria)]
            );
        }

        if (CompletionCriteria != TaskCompletionCriteria.REPETITIONS && RepetitionCriteria is not null)
        {
            yield return new ValidationResult(
                "RepetitionCriteria solo debe enviarse cuando el criterio de completado es REPETITIONS.",
                [nameof(RepetitionCriteria)]
            );
        }

        if (CompletionCriteria == TaskCompletionCriteria.TIMER && TimerCriteria is null)
        {
            yield return new ValidationResult(
                "Los criterios de timer son obligatorios cuando el criterio de completado es TIMER.",
                [nameof(TimerCriteria)]
            );
        }

        if (CompletionCriteria != TaskCompletionCriteria.TIMER && TimerCriteria is not null)
        {
            yield return new ValidationResult(
                "TimerCriteria solo debe enviarse cuando el criterio de completado es TIMER.",
                [nameof(TimerCriteria)]
            );
        }

        if (CompletionCriteria != TaskCompletionCriteria.EVIDENCE && Evidence is not null)
        {
            yield return new ValidationResult(
                "Evidence solo debe enviarse cuando el criterio de completado es EVIDENCE.",
                [nameof(Evidence)]
            );
        }
    }
}
