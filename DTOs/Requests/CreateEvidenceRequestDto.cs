using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LevelUpLifeBackend.DTOs.Requests;

public class CreateEvidenceRequestDto : IValidatableObject
{
    [JsonPropertyName("url")]
    public string? EvidencePathUrl { get; set; }

    [JsonPropertyName("healthDataJson")]
    public string? HealthDataJson { get; set; }

    [Required(ErrorMessage = "uploadedAt is required.")]
    [JsonPropertyName("uploadedAt")]
    public DateTime? UploadedAt { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(EvidencePathUrl) && string.IsNullOrWhiteSpace(HealthDataJson))
        {
            yield return new ValidationResult(
                "Either url or healthDataJson must be provided.",
                [nameof(EvidencePathUrl), nameof(HealthDataJson)]
            );
        }
    }
}
