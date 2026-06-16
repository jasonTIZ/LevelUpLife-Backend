using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LevelUpLifeBackend.DTOs.Requests;

public class UpdateHabitDisciplineStatusRequestDto
{
    [Required(ErrorMessage = "statusHabitDisciplineIsActive is required.")]
    [JsonPropertyName("statusHabitDisciplineIsActive")]
    [Display(Name = "statusHabitDisciplineIsActive")]
    public bool StatusHabitDisciplineIsActive { get; set; }
}
