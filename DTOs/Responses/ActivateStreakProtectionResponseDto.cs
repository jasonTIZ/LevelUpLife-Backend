namespace LevelUpLifeBackend.DTOs.Responses;

public class ActivateStreakProtectionResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int RemainingProtectionsThisMonth { get; set; }
}
