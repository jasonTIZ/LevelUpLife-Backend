namespace LevelUpLifeBackend.DTOs.Responses;

public class HabitTaskPagedResponseDto
{
    public IEnumerable<HabitTaskResponseDto> Data { get; set; } = [];
    public int Page { get; set; }
    public int Size { get; set; }
    public int Total { get; set; }
}
