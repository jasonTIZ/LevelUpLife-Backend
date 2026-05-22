using System.ComponentModel.DataAnnotations;
using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.DTOs.Requests;

public class HabitTaskListQueryDto
{
    public int? HabitId { get; set; }
    public int? DisciplineId { get; set; }
    public TaskDifficulty? Difficulty { get; set; }
    public TaskFrequency? Frequency { get; set; }
    public bool? IsActive { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "page must be greater than or equal to 0.")]
    public int Page { get; set; } = 0;

    [Range(1, 100, ErrorMessage = "size must be between 1 and 100.")]
    public int Size { get; set; } = 10;
}
