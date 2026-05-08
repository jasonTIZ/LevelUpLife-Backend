using LevelUpLifeBackend.Infrastructure.Errors;
using LevelUpLifeBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace LevelUpLifeBackend.Controllers;

[ApiController]
[Route("api/habit-tasks")]
public class HabitTaskController : ControllerBase
{
    private readonly IHabitTaskService _habitTaskService;

    public HabitTaskController(IHabitTaskService habitTaskService)
    {
        _habitTaskService = habitTaskService;
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
}
