namespace LevelUpLifeBackend.DTOs.Responses;

public class HabitResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DisciplineId { get; set; }
    public int UserId { get; set; }
    public bool IsActive { get; set; }
}
