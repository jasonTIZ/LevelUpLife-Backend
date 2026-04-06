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
}
