using System.ComponentModel.DataAnnotations;

namespace LevelUpLifeBackend.DTOs.Requests;

public class CompleteHabitTaskRequestDto
{
    [Required(ErrorMessage = "completedAt is required.")]
    public DateTime CompletedAt { get; set; }
}
