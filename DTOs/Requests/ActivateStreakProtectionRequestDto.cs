using System.ComponentModel.DataAnnotations;
using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.DTOs.Requests;

public class ActivateStreakProtectionRequestDto
{
    [Required]
    public StreakProtectionType Type { get; set; }
}
