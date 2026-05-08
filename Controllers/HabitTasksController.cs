using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.Infrastructure.Errors;
using LevelUpLifeBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace LevelUpLifeBackend.Controllers;

[ApiController]
[Route("api/habit-tasks")]
public class HabitTasksController : ControllerBase
{
    private readonly IHabitService _habitService;

    public HabitTasksController(IHabitService habitService)
    {
        _habitService = habitService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStandaloneHabitTaskRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var userId = ResolveUserId();
        if (userId is null)
        {
            return Unauthorized(
                new
                {
                    code = "UNAUTHORIZED",
                    message = "Unauthorized access",
                    details = "A valid JWT or X-User-Id header is required.",
                }
            );
        }

        try
        {
            var created = await _habitService.CreateTaskAsync(request, userId.Value);
            return StatusCode(StatusCodes.Status201Created, created);
        }
        catch (ConflictError ex) when (ex.Payload.Message == "TASK_ALREADY_EXISTS")
        {
            return Conflict(
                new
                {
                    code = "TASK_ALREADY_EXISTS",
                    message = "Task already exists for this habit",
                    details = ex.Payload.Details,
                }
            );
        }
        catch (NotFoundError ex)
        {
            return NotFound(
                new
                {
                    code = "HABIT_NOT_FOUND",
                    message = ex.Payload.Message,
                    details = ex.Payload.Details,
                }
            );
        }
        catch (AuthError ex) when (ex.Kind == AuthFailureKind.Forbidden)
        {
            return StatusCode(
                StatusCodes.Status403Forbidden,
                new
                {
                    code = "FORBIDDEN",
                    message = ex.Payload.Message,
                    details = ex.Payload.Details,
                }
            );
        }
        catch (Exception)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    code = "SERVER_ERROR",
                    message = "Internal server error",
                    details = "An unexpected error occurred while creating the habit task.",
                }
            );
        }
    }

    private int? ResolveUserId()
    {
        var headerValue = Request.Headers["X-User-Id"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(headerValue) && int.TryParse(headerValue, out var headerUserId))
        {
            return headerUserId;
        }

        var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (!string.IsNullOrWhiteSpace(claimValue) && int.TryParse(claimValue, out var claimUserId))
        {
            return claimUserId;
        }

        return null;
    }
}
