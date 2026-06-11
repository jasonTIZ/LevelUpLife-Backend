using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LevelUpLifeBackend.DTOs.Requests;

public class CreateEvidenceRequestDto : IValidatableObject
{
    [JsonPropertyName("DSC_EVIDENCE_PATH_URL")]
    public string? EvidencePathUrl { get; set; }

    [JsonPropertyName("DSC_HEALTH_DATA_JSON")]
    public string? HealthDataJson { get; set; }

    [Required(ErrorMessage = "FEC_UPLOADED is required.")]
    [JsonPropertyName("FEC_UPLOADED")]
    public DateTime? UploadedAt { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(EvidencePathUrl) && string.IsNullOrWhiteSpace(HealthDataJson))
        {
            yield return new ValidationResult(
                "Either DSC_EVIDENCE_PATH_URL or DSC_HEALTH_DATA_JSON must be provided.",
                [nameof(EvidencePathUrl), nameof(HealthDataJson)]
            );
        }
    }
}
