using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.DTOs.Responses;

namespace LevelUpLifeBackend.Services;

public interface IAuthService
{
    // Retorna null si las credenciales son inválidas.
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);

    Task<(bool Success, string Message)> RegisterAsync(RegisterRequestDto request);
}
