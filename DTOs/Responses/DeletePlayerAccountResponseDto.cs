namespace LevelUpLifeBackend.DTOs.Responses;

public class DeletePlayerAccountResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime DeactivatedAt { get; set; }
}
