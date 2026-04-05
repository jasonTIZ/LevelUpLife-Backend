using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.Services;
using Microsoft.AspNetCore.Mvc;

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
        var createdHabit = await _habitService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = createdHabit.Id }, createdHabit);
    }
}
