using System.Text.Json.Serialization;

namespace LevelUpLifeBackend.DTOs.Responses;

public class LevelProgressDto
{
    [JsonPropertyName("currentLevel")]
    public int CurrentLevel { get; set; }

    [JsonPropertyName("totalExperiencePoints")]
    public int TotalExperiencePoints { get; set; }

    [JsonPropertyName("experiencePointsInCurrentLevel")]
    public int ExperiencePointsInCurrentLevel { get; set; }

    [JsonPropertyName("experiencePointsRequiredForNextLevel")]
    public int ExperiencePointsRequiredForNextLevel { get; set; }

    [JsonPropertyName("levelProgressPercent")]
    public double LevelProgressPercent { get; set; }
}
