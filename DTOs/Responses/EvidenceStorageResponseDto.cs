using System.Text.Json.Serialization;

namespace LevelUpLifeBackend.DTOs.Responses;

public class EvidenceStorageResponseDto
{
    [JsonPropertyName("ID_EVIDENCE")]
    public int Id { get; set; }

    [JsonPropertyName("ID_HABIT_TASK")]
    public int HabitTaskId { get; set; }

    [JsonPropertyName("DSC_EVIDENCE_PATH_URL")]
    public string? EvidencePathUrl { get; set; }

    [JsonPropertyName("DSC_HEALTH_DATA_JSON")]
    public string? HealthDataJson { get; set; }

    [JsonPropertyName("FEC_UPLOADED")]
    public DateTime UploadedAt { get; set; }
}
