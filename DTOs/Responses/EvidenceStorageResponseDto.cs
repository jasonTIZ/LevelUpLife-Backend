using System.Text.Json.Serialization;

namespace LevelUpLifeBackend.DTOs.Responses;

public class EvidenceStorageResponseDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("habitTaskId")]
    public int HabitTaskId { get; set; }

    [JsonPropertyName("url")]
    public string? EvidencePathUrl { get; set; }

    [JsonPropertyName("healthDataJson")]
    public string? HealthDataJson { get; set; }

    [JsonPropertyName("uploadedAt")]
    public DateTime UploadedAt { get; set; }
}
