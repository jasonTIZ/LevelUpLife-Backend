using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.Services;
using LevelUpLifeBackend.Infrastructure.Http.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using LevelUpLifeBackend.Infrastructure.Http;

namespace LevelUpLifeBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ISessionResetService _sessionResetService;
    private readonly ISecureCredentialStorage _credentialStorage;
    private readonly BaseHttpClientOptions _httpOptions;

    public AuthController(
        IAuthService authService, 
        ISessionResetService sessionResetService, 
        ISecureCredentialStorage credentialStorage,
        IOptions<BaseHttpClientOptions> httpOptions)
    {
        _authService = authService;
        _sessionResetService = sessionResetService;
        _credentialStorage = credentialStorage;
        _httpOptions = httpOptions.Value;
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

        // Intentamos obtener el SESSION_ID de la petición actual (cookie o header)
        var sessionId = Request.Cookies[_httpOptions.SessionCookieName];
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            sessionId = Request.Headers[_httpOptions.SessionCookieName].ToString();
        }

        // PERSISTENCIA: Guardamos las credenciales en el almacenamiento seguro (Cookie cifrada)
        _credentialStorage.SaveCredentials(result.Token, sessionId ?? string.Empty);

        return Ok(new { success = true, data = result });
    }

    // POST api/auth/logout
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _sessionResetService.ResetAsync();
        return Ok(new { success = true, message = "Sesión cerrada correctamente." });
    }

    // POST api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        var (success, message) = await _authService.RegisterAsync(request);

        if (!success)
            return Conflict(new { code = "DUPLICATE_ACCOUNT", message });

        return StatusCode(201, new { success = true, message });
    }
}
