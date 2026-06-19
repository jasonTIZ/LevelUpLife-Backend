using System.Text.Json.Serialization;

namespace LevelUpLifeBackend.DTOs.Responses;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public int Level { get; set; }
    public string ClassName { get; set; } = string.Empty;

    [JsonPropertyName("levelProgress")]
    public LevelProgressDto LevelProgress { get; set; } = new();

    [JsonPropertyName("levelingConfig")]
    public LevelingConfigDto LevelingConfig { get; set; } = new();
}
