using System.ComponentModel.DataAnnotations;

namespace LevelUpLifeBackend.DTOs.Requests;

public class UpdateHabitTaskRequestDto
{
    [Required(ErrorMessage = "El identificador de la tarea es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "La tarea no es válida.")]
    public int TaskId { get; set; }

    public UpdateRepetitionCriteriaRequestDto? RepetitionCriteria { get; set; }
}
