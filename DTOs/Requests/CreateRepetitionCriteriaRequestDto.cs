using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.DTOs.Requests;

public class CreateRepetitionCriteriaRequestDto
{
    [JsonPropertyName("NUM_REPETITIONS_OBJECTIVE")]
    [Required(ErrorMessage = "La cantidad de repeticiones es obligatoria.")]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad de repeticiones debe ser mayor a 0.")]
    public int? NumRepetitionsObjective { get; set; }

    [JsonPropertyName("TYPE_UNITY_MEASUREMENT_UNIT")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Required(ErrorMessage = "La unidad de medida es obligatoria.")]
    public MeasurementUnit? TypeUnityMeasurementUnit { get; set; }

    [JsonPropertyName("STATUS_IS_PARTIAL_ALLOWED")]
    public bool? StatusIsPartialAllowed { get; set; }

    [JsonPropertyName("STATUS_REPETITIONS_CRITERIA_IS_ACTIVE")]
    public bool? StatusRepetitionsCriteriaIsActive { get; set; }
}