using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.Infrastructure.Errors;
using LevelUpLifeBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LevelUpLifeBackend.Controllers;

[ApiController]
[Route("api/habit-tasks")]
public class HabitTaskController : ControllerBase
{
    private readonly IHabitService _habitService;
    private readonly IHabitTaskService _habitTaskService;
    private readonly IRepetitionCriteriaService _repetitionCriteriaService;
    private readonly ITimerCriteriaService _timerCriteriaService;

    public HabitTaskController(
        IHabitService habitService,
        IHabitTaskService habitTaskService,
        IRepetitionCriteriaService repetitionCriteriaService,
        ITimerCriteriaService timerCriteriaService)
    {
        _habitService = habitService;
        _habitTaskService = habitTaskService;
        _repetitionCriteriaService = repetitionCriteriaService;
        _timerCriteriaService = timerCriteriaService;
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

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateStandaloneHabitTaskRequestDto request)
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
            var updated = await _habitService.UpdateTaskAsync(id, request, userId.Value);
            return Ok(updated);
        }
        catch (NotFoundError ex)
        {
            return NotFound(
                new
                {
                    code = "TASK_NOT_FOUND",
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
        catch (AppError ex) when (ex.HttpStatusCode == StatusCodes.Status400BadRequest)
        {
            return BadRequest(
                new
                {
                    code = "VALIDATION_ERROR",
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
                    details = "An unexpected error occurred while updating the habit task.",
                }
            );
        }
    }

    [HttpGet("{taskId:int}")]
    public async Task<IActionResult> GetById(int taskId)
    {
        try
        {
            var task = await _habitTaskService.GetByIdAsync(taskId);
            return Ok(task);
        }
        catch (NotFoundError ex)
        {
            return NotFound(ex.Payload);
        }
        catch (ServerError ex)
        {
            return StatusCode(ex.HttpStatusCode, ex.Payload);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse
            {
                Code = 500,
                Message = "An unexpected error occurred.",
                Details = ex.Message
            });
        }
    }

    [HttpPost("{taskId:int}/evidences")]
    public async Task<IActionResult> CreateEvidence(int taskId, [FromBody] CreateEvidenceRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            var created = await _habitTaskService.CreateEvidenceAsync(taskId, request);
            return StatusCode(StatusCodes.Status201Created, created);
        }
        catch (NotFoundError ex)
        {
            return NotFound(ex.Payload);
        }
        catch (ServerError ex)
        {
            return StatusCode(ex.HttpStatusCode, ex.Payload);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse
            {
                Code = 500,
                Message = "An unexpected error occurred.",
                Details = ex.Message
            });
        }
    }

    [HttpGet("{taskId:int}/evidences")]
    public async Task<IActionResult> GetEvidences(int taskId)
    {
        try
        {
            var evidences = await _habitTaskService.GetEvidencesByTaskIdAsync(taskId);
            return Ok(evidences);
        }
        catch (NotFoundError ex)
        {
            return NotFound(ex.Payload);
        }
        catch (ServerError ex)
        {
            return StatusCode(ex.HttpStatusCode, ex.Payload);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse
            {
                Code = 500,
                Message = "An unexpected error occurred.",
                Details = ex.Message
            });
        }
    }

    [Authorize]
    [HttpDelete("{taskId:int}/timer-criteria/{id:int}")]
    public async Task<IActionResult> DeactivateTimerCriteria(int taskId, int id)
    {
        try
        {
            await _timerCriteriaService.DeactivateAsync(taskId, id);
            return Ok(new { message = "Timer criteria deactivated successfully." });
        }
        catch (NotFoundError ex)
        {
            return NotFound(ex.Payload);
        }
        catch (ServerError ex)
        {
            return StatusCode(ex.HttpStatusCode, ex.Payload);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse
            {
                Code = 500,
                Message = "An unexpected error occurred.",
                Details = ex.Message
            });
        }
    }

    [HttpDelete("{taskId:int}/repetition-criteria/{id:int}")]
    public async Task<IActionResult> DeactivateRepetitionCriteria(int taskId, int id)
    {
        try
        {
            await _repetitionCriteriaService.DeactivateAsync(taskId, id);
            return Ok(new { message = "Repetition criteria deactivated successfully." });
        }
        catch (NotFoundError ex)
        {
            return NotFound(ex.Payload);
        }
        catch (ServerError ex)
        {
            return StatusCode(ex.HttpStatusCode, ex.Payload);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse
            {
                Code = 500,
                Message = "An unexpected error occurred.",
                Details = ex.Message
            });
        }
    }

    [HttpGet("{taskId:int}/evidences/{id:int}")]
    public async Task<IActionResult> GetEvidenceById(int taskId, int id)
    {
        try
        {
            var evidence = await _habitTaskService.GetEvidenceByIdAsync(taskId, id);
            return Ok(evidence);
        }
        catch (NotFoundError ex)
        {
            return NotFound(ex.Payload);
        }
        catch (ServerError ex)
        {
            return StatusCode(ex.HttpStatusCode, ex.Payload);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse
            {
                Code = 500,
                Message = "An unexpected error occurred.",
                Details = ex.Message
            });
        }
    }

    [HttpDelete("{taskId:int}/evidences/{id:int}")]
    public async Task<IActionResult> DeleteEvidence(int taskId, int id)
    {
        try
        {
            await _habitTaskService.DeleteEvidenceAsync(taskId, id);
            return Ok(new { message = "Evidence deleted successfully." });
        }
        catch (NotFoundError ex)
        {
            return NotFound(ex.Payload);
        }
        catch (ServerError ex)
        {
            return StatusCode(ex.HttpStatusCode, ex.Payload);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse
            {
                Code = 500,
                Message = "An unexpected error occurred.",
                Details = ex.Message
            });
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
