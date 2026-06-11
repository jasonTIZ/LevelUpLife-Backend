namespace LevelUpLifeBackend.DTOs.Responses;

public class UpdatePlayerProfileResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public PlayerProfileDto Player { get; set; } = new();
}

public class PlayerProfileDto
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int Level { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastLogin { get; set; }
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public PersonProfileDto Person { get; set; } = new();
}

public class PersonProfileDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateOnly? Birthdate { get; set; }
    public bool IsActive { get; set; }
}
