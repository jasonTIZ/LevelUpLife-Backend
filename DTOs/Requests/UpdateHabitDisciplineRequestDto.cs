using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LevelUpLifeBackend.DTOs.Requests;

public class UpdateHabitDisciplineRequestDto
{
    [Required(ErrorMessage = "idHabitCategory is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "idHabitCategory must be greater than 0.")]
    [JsonPropertyName("idHabitCategory")]
    [Display(Name = "idHabitCategory")]
    public int IdHabitCategory { get; set; }

    [Required(ErrorMessage = "dscHabitDisciplineName is required.")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "dscHabitDisciplineName must be between 1 and 50 characters.")]
    [JsonPropertyName("dscHabitDisciplineName")]
    [Display(Name = "dscHabitDisciplineName")]
    public string DscHabitDisciplineName { get; set; } = string.Empty;

    [JsonPropertyName("dscHabitDisciplineDescription")]
    [Display(Name = "dscHabitDisciplineDescription")]
    public string? DscHabitDisciplineDescription { get; set; }

    [JsonPropertyName("statusHabitDisciplineIsActive")]
    [Display(Name = "statusHabitDisciplineIsActive")]
    public bool StatusHabitDisciplineIsActive { get; set; } = true;
}
