using System.ComponentModel.DataAnnotations;

namespace LevelUpLifeBackend.DTOs.Requests;

public class ActivateStreakProtectionRequestDto
{
    [Required]
    public string Type { get; set; } = string.Empty;
}
