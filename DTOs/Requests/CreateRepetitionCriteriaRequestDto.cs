using System.ComponentModel.DataAnnotations;
using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.DTOs.Requests;

public class CreateRepetitionCriteriaRequestDto
{
    [Required(ErrorMessage = "La cantidad de repeticiones es obligatoria.")]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad de repeticiones debe ser mayor a 0.")]
    public int? Repetitions { get; set; }

    [Required(ErrorMessage = "La unidad de medida es obligatoria.")]
    public MeasurementUnit? MeasurementUnit { get; set; }

    public bool? IsPartialAllowed { get; set; }

    public bool? IsActive { get; set; }
}
