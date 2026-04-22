using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace LevelUpLifeBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // POST api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var result = await _authService.LoginAsync(request);

        // Si result es null, las credenciales son incorrectas.
        // Usamos 401 (no 404) para no revelar si el usuario existe.
        if (result is null)
            return Unauthorized(new { success = false, message = "Credenciales inválidas." });

        return Ok(new { success = true, data = result });
    }

    // POST api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        var (success, message) = await _authService.RegisterAsync(request);

        if (!success)
            return Conflict(new { success = false, message });

        return Ok(new { success = true, message });
    }
}
