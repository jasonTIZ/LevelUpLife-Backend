using System.Security.Claims;
using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LevelUpLifeBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HabitsController : ControllerBase
{
    private readonly IHabitService _habitService;

    public HabitsController(IHabitService habitService)
    {
        _habitService = habitService;
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
                    new
                    {
                        success = false,
                        message = "No se encontraron hábitos para el usuario.",
                    }
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
}
