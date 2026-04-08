namespace LevelUpLifeBackend.DTOs.Requests;

// Lo que el frontend envía al hacer login.
// UserNameOrEmail acepta tanto el UserName como el Email de Person.
public class LoginRequestDto
{
    public string UserNameOrEmail { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
