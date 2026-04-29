using System.ComponentModel.DataAnnotations;

namespace LevelUpLifeBackend.DTOs.Requests;

public class CreateHabitTaskRequestDto
{
    [Required(ErrorMessage = "Los criterios de repetición de la tarea son obligatorios.")]
    public CreateRepetitionCriteriaRequestDto? RepetitionCriteria { get; set; }
}
