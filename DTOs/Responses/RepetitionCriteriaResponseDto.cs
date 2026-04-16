using System.Text.Json.Serialization;
using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.DTOs.Responses;

public class RepetitionCriteriaResponseDto
{
    [JsonPropertyName("ID_HABIT_TASK_REPETITIONS_CRITERIA")]
    public int Id { get; set; }

    [JsonPropertyName("ID_HABIT_TASK")]
    public int HabitTaskId { get; set; }

    [JsonPropertyName("NUM_REPETITIONS_OBJECTIVE")]
    public int NumRepetitionsObjective { get; set; }

    [JsonPropertyName("TYPE_UNITY_MEASUREMENT_UNIT")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public MeasurementUnit TypeUnityMeasurementUnit { get; set; }

    [JsonPropertyName("STATUS_IS_PARTIAL_ALLOWED")]
    public bool StatusIsPartialAllowed { get; set; }

    [JsonPropertyName("STATUS_REPETITIONS_CRITERIA_IS_ACTIVE")]
    public bool StatusRepetitionsCriteriaIsActive { get; set; }
}