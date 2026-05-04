using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.DTOs.Responses;

public class RepetitionCriteriaResponseDto
{
    public int Id { get; set; }
    public int HabitTaskId { get; set; }
    public int Repetitions { get; set; }
    public MeasurementUnit MeasurementUnit { get; set; }
    public bool IsPartialAllowed { get; set; }
    public bool IsActive { get; set; }
}
