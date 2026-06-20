using System.Text.Json.Serialization;

namespace LevelUpLifeBackend.DTOs.Responses;

public class GetPlayerProfileResponseDto
{
    [JsonPropertyName("PlayerUserId")]
    public string PlayerUserId { get; set; } = string.Empty;

    [JsonPropertyName("PlayerUserUserName")]
    public string PlayerUserUserName { get; set; } = string.Empty;

    [JsonPropertyName("PlayerUserLevel")]
    public int PlayerUserLevel { get; set; }

    [JsonPropertyName("totalExperiencePoints")]
    public int TotalExperiencePoints { get; set; }

    [JsonPropertyName("experiencePointsInCurrentLevel")]
    public int ExperiencePointsInCurrentLevel { get; set; }

    [JsonPropertyName("experiencePointsRequiredForNextLevel")]
    public int ExperiencePointsRequiredForNextLevel { get; set; }

    [JsonPropertyName("levelProgressPercent")]
    public double LevelProgressPercent { get; set; }

    [JsonPropertyName("daysStreak")]
    public int DaysStreak { get; set; }

    [JsonPropertyName("levelingConfig")]
    public LevelingConfigDto LevelingConfig { get; set; } = new();

    [JsonPropertyName("statusIsActive")]
    public bool StatusIsActive { get; set; }

    [JsonPropertyName("PlayerUserLastLogin")]
    public DateTime? PlayerUserLastLogin { get; set; }

    [JsonPropertyName("PlayerUserCreationDate")]
    public DateTime PlayerUserCreationDate { get; set; }

    [JsonPropertyName("bio")]
    public string? Bio { get; set; }

    [JsonPropertyName("avatarUrl")]
    public string? AvatarUrl { get; set; }

    [JsonPropertyName("gold")]
    public int Gold { get; set; }

    [JsonPropertyName("PersonData")]
    public GetPlayerProfilePersonDataDto PersonData { get; set; } = new();
}

public class GetPlayerProfilePersonDataDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("birthdate")]
    public DateOnly? Birthdate { get; set; }
}
