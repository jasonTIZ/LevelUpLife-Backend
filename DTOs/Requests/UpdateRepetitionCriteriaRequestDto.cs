using System.ComponentModel.DataAnnotations;
using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.DTOs.Requests;

public class UpdateRepetitionCriteriaRequestDto
{
    [Required]
    public int Repetitions { get; set; }

    [Required]
    public MeasurementUnit MeasurementUnit { get; set; }

    public bool? IsPartialAllowed { get; set; }
    public bool? IsActive { get; set; }
}
