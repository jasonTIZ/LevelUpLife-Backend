using System.Text.Json.Serialization;

namespace LevelUpLifeBackend.DTOs.Responses;

public sealed class HabitDisciplineDetailResponseDto
{
    [JsonPropertyName("idHabitDiscipline")]
    public int IdHabitDiscipline { get; set; }

    [JsonPropertyName("idHabitCategory")]
    public int IdHabitCategory { get; set; }

    [JsonPropertyName("dscHabitDisciplineName")]
    public string DscHabitDisciplineName { get; set; } = string.Empty;

    [JsonPropertyName("dscHabitDisciplineDescription")]
    public string DscHabitDisciplineDescription { get; set; } = string.Empty;

    [JsonPropertyName("statusHabitDisciplineIsActive")]
    public bool StatusHabitDisciplineIsActive { get; set; }
}
