namespace LevelUpLifeBackend.DTOs.Responses;

// Lo que el backend devuelve tras un login exitoso.
public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public int Level { get; set; }
    public string ClassName { get; set; } = string.Empty;
}
