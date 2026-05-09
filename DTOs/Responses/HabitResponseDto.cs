namespace LevelUpLifeBackend.DTOs.Responses;

public class HabitResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DisciplineName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<HabitTaskResponseDto> Tasks { get; set; } = [];
}
