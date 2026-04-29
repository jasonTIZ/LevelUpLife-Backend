using System.ComponentModel.DataAnnotations;

namespace LevelUpLifeBackend.DTOs.Requests;

public class DeletePlayerAccountRequestDto
{
    [StringLength(200)]
    public string? Reason { get; set; }
}
