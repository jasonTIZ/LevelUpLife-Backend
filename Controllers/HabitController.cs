using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.Infrastructure.Errors;
using LevelUpLifeBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LevelUpLifeBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HabitsController : ControllerBase
{
    private readonly IHabitService _habitService;
    private readonly IHabitTaskService _habitTaskService;

    public HabitsController(IHabitService habitService, IHabitTaskService habitTaskService)
    {
        _habitService = habitService;
        _habitTaskService = habitTaskService;
    }

    [Authorize]
    [HttpGet("{habitId:int}/tasks")]
    public async Task<IActionResult> ListHabitTasks(
        int habitId,
        [FromQuery] HabitTaskListQueryDto query
    )
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
                code = "UNAUTHORIZED",
                message = "Unauthorized access",
                details = "A valid JWT is required.",
            });
        }

        try
        {
            var result = await _habitTaskService.ListByHabitAsync(habitId, userId.Value, query);
            return Ok(result);
        }
        catch (NotFoundError ex)
        {
            return NotFound(new
            {
                code = "HABIT_NOT_FOUND",
                message = ex.Payload.Message,
                details = ex.Payload.Details,
            });
        }
        catch (ServerError ex)
        {
            return StatusCode(ex.HttpStatusCode, ex.Payload);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var habit = await _habitService.GetByIdAsync(id);

        if (habit is null)
        {
            return NotFound();
        }
        return Ok(habit);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateHabitRequestDto request)
    {
        try
        {
            var createdHabit = await _habitService.CreateAsync(request);

            if (createdHabit is null)
            {
                return BadRequest(
                    new
                    {
                        success = false,
                        message = "No se pudo crear el hábito. Verifica los datos enviados.",
                    }
                );
            }

            var customResponse = new
            {
                success = true,
                message = "El nuevo hábito se ha creado exitosamente.",
            };
            return CreatedAtAction(nameof(GetById), new { id = createdHabit.Id }, customResponse);
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine($"Error de BD: {ex.InnerException?.Message}");

            return BadRequest(
                new
                {
                    success = false,
                    message = "Error guardando en la base de datos. Verifica que el Usuario y la Disciplina existan.",
                }
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error Inesperado: {ex.Message}");

            return StatusCode(
                500,
                new
                {
                    success = false,
                    message = "Ocurrió un error inesperado al procesar tu solicitud.",
                }
            );
        }
    }

    // [Authorize] //
    [HttpGet("active")]
    public async Task<IActionResult> GetActiveHabits(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10
    )
    {
        try
        {
            //var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userIdString = Request.Headers["X-User-Id"].FirstOrDefault();

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized(new { success = false, message = "Acceso no autorizado." });
            }

            if (pageNumber < 1)
                pageNumber = 1;
            if (pageSize < 1 || pageSize > 100)
                pageSize = 10;

            var pagedResult = await _habitService.GetActiveHabitsPaginatedAsync(
                pageNumber,
                pageSize,
                userId
            );

            if (pagedResult.Items == null || !pagedResult.Items.Any())
            {
                return BadRequest(
                    new { success = false, message = "No se encontraron hábitos para el usuario." }
                );
            }

            return Ok(
                new
                {
                    success = true,
                    data = pagedResult.Items,
                    pagination = new
                    {
                        currentPage = pagedResult.CurrentPage,
                        pageSize = pagedResult.PageSize,
                        totalPages = pagedResult.TotalPages,
                        totalRecords = pagedResult.TotalRecords,
                    },
                }
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error inesperado: {ex.Message}");
            return StatusCode(
                500,
                new
                {
                    success = false,
                    message = "Ocurrió un error inesperado al procesar la solicitud.",
                }
            );
        }
    }

    // [Authorize] //
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateHabitRequestDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest(
                new { success = false, message = "El ID de la ruta no coincide con el del cuerpo." }
            );
        }

        try
        {
            //var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userIdString = Request.Headers["X-User-Id"].FirstOrDefault();

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized(new { success = false, message = "Acceso no autorizado." });
            }

            var updatedHabit = await _habitService.UpdateHabitAsync(dto);

            if (updatedHabit == null)
            {
                return NotFound(
                    new { success = false, message = $"El hábito que desea editar no existe." }
                );
            }

            return Ok(
                new
                {
                    success = true,
                    message = "Hábito actualizado exitosamente!",
                    data = updatedHabit,
                }
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al actualizar: {ex.Message}");
            return StatusCode(
                500,
                new
                {
                    success = false,
                    message = "Ocurrió un error inesperado al intentar actualizar el hábito.",
                }
            );
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
