using System.ComponentModel.DataAnnotations;
using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.DTOs.Requests;

public class CreateHabitTaskRequestDto
{
    [Required(ErrorMessage = "El título de la tarea es obligatorio.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El título debe tener entre 3 y 100 caracteres.")]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

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

    [Required(ErrorMessage = "Los criterios de repetición de la tarea son obligatorios.")]
    public CreateRepetitionCriteriaRequestDto? RepetitionCriteria { get; set; }
}
