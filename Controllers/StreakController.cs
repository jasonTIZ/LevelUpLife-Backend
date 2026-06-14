using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.Infrastructure.Errors;
using LevelUpLifeBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LevelUpLifeBackend.Controllers;

[ApiController]
[Route("api/streaks")]
[Authorize]
public class StreakController : ControllerBase
{
    private readonly IStreakService _streakService;

    public StreakController(IStreakService streakService)
    {
        _streakService = streakService;
    }

    /// <summary>
    /// Activa protección de racha para el día actual (UTC).
    /// Tipos: TRABAJO, EVALUACION, EMERGENCIA. Límite mensual configurable.
    /// </summary>
    [HttpPost("protection")]
    public async Task<IActionResult> ActivateProtection(
        [FromBody] ActivateStreakProtectionRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var userId = ResolveAuthenticatedUserId();
        if (userId is null)
        {
            return Unauthorized(new
            {
                success = false,
                message = "Unauthorized access",
                details = "A valid Bearer JWT is required.",
            });
        }

        try
        {
            var result = await _streakService.ActivateProtectionAsync(userId.Value, request);
            return Ok(result);
        }
        catch (NotFoundError ex)
        {
            return NotFound(new
            {
                success = false,
                message = ex.Payload.Message,
                details = ex.Payload.Details,
            });
        }
        catch (StreakError ex) when (ex.Kind == StreakFailureKind.ProtectionLimitExceeded)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new
            {
                success = false,
                message = ex.Payload.Message,
                details = ex.Payload.Details,
            });
        }
        catch (StreakError ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Payload.Message,
                details = ex.Payload.Details,
            });
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                success = false,
                message = "Internal server error",
                details = "An unexpected error occurred while activating streak protection.",
            });
        }
    }

    private int? ResolveAuthenticatedUserId()
    {
        var userIdValue =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (string.IsNullOrWhiteSpace(userIdValue) || !int.TryParse(userIdValue, out int playerUserId))
        {
            return null;
        }

        return playerUserId;
    }
}
