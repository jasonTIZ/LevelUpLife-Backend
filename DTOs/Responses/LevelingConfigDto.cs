using System.Text.Json.Serialization;

namespace LevelUpLifeBackend.DTOs.Responses;

public class LevelingConfigDto
{
    [JsonPropertyName("strategy")]
    public string Strategy { get; set; } = "escalating_percent";

    [JsonPropertyName("baseXpPerLevel")]
    public int BaseXpPerLevel { get; set; }

    [JsonPropertyName("escalationPercent")]
    public int EscalationPercent { get; set; }
}
