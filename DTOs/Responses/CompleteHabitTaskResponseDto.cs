using System.Text.Json.Serialization;

namespace LevelUpLifeBackend.DTOs.Responses;

public class CompleteHabitTaskResponseDto
{
    [JsonPropertyName("xpEarned")]
    public int XpEarned { get; set; }

    [JsonPropertyName("previousLevel")]
    public int PreviousLevel { get; set; }

    [JsonPropertyName("newLevel")]
    public int NewLevel { get; set; }

    [JsonPropertyName("totalExperiencePoints")]
    public int TotalExperiencePoints { get; set; }

    [JsonPropertyName("experiencePointsInCurrentLevel")]
    public int ExperiencePointsInCurrentLevel { get; set; }

    [JsonPropertyName("experiencePointsRequiredForNextLevel")]
    public int ExperiencePointsRequiredForNextLevel { get; set; }

    [JsonPropertyName("levelProgressPercent")]
    public double LevelProgressPercent { get; set; }

    [JsonPropertyName("leveledUp")]
    public bool LeveledUp { get; set; }

    [JsonPropertyName("streakUpdated")]
    public bool StreakUpdated { get; set; }

    [JsonPropertyName("daysStreak")]
    public int DaysStreak { get; set; }

    [JsonPropertyName("levelingConfig")]
    public LevelingConfigDto LevelingConfig { get; set; } = new();
}
