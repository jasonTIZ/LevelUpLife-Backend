using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.Exceptions;
using LevelUpLifeBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace LevelUpLifeBackend.Controllers;

[ApiController]
[Route("habit-tasks")]
public class HabitTasksController : ControllerBase
{
    private readonly IRepetitionCriteriaService _repetitionCriteriaService;

    public HabitTasksController(IRepetitionCriteriaService repetitionCriteriaService)
    {
        _repetitionCriteriaService = repetitionCriteriaService;
    }

    [HttpPost("{taskId:int}/repetition-criteria")]
    public async Task<IActionResult> CreateRepetitionCriteria(
        [FromRoute] int taskId,
        [FromBody] CreateRepetitionCriteriaRequestDto request
    )
    {
        if (taskId < 1)
        {
            ModelState.AddModelError("taskId", "El identificador de la tarea es obligatorio.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                success = false,
                message = "La solicitud contiene errores de validación.",
                errors = ModelState
                    .Where(entry => entry.Value?.Errors.Count > 0)
                    .ToDictionary(
                        entry => entry.Key,
                        entry => entry.Value!.Errors.Select(error => error.ErrorMessage).ToArray()
                    ),
            });
        }

        try
        {
            var createdCriteria = await _repetitionCriteriaService.CreateAsync(taskId, request);

            if (createdCriteria is null)
            {
                return NotFound(new
                {
                    success = false,
                    code = "TASK_NOT_FOUND",
                    message = "La tarea no existe.",
                });
            }

            return StatusCode(StatusCodes.Status201Created, createdCriteria);
        }
        catch (RepetitionCriteriaAlreadyExistsException ex)
        {
            return Conflict(new
            {
                success = false,
                code = RepetitionCriteriaAlreadyExistsException.ErrorCode,
                message = ex.Message,
            });
        }
    }
}