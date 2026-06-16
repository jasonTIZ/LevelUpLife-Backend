using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.Infrastructure.Errors;
using LevelUpLifeBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LevelUpLifeBackend.Controllers;

[ApiController]
[Route("api/habit/disciplines")]
public class HabitDisciplineApiController : ControllerBase
{
    private readonly IHabitDisciplineService _habitDisciplineService;

    public HabitDisciplineApiController(IHabitDisciplineService habitDisciplineService)
    {
        _habitDisciplineService = habitDisciplineService;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var disciplines = await _habitDisciplineService.GetAllDisciplinesAsync();
            return Ok(disciplines);
        }
        catch (ServerError ex)
        {
            return StatusCode(ex.HttpStatusCode, ex.Payload);
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                new ErrorResponse
                {
                    Code = 500,
                    Message = "An unexpected error occurred.",
                    Details = ex.Message,
                }
            );
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var discipline = await _habitDisciplineService.GetDisciplineByIdAsync(id);
            return Ok(discipline);
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
            return StatusCode(
                500,
                new ErrorResponse
                {
                    Code = 500,
                    Message = "An unexpected error occurred.",
                    Details = ex.Message,
                }
            );
        }
    }

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateHabitDisciplineRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            var updated = await _habitDisciplineService.UpdateDisciplineAsync(id, request);
            return Ok(updated);
        }
        catch (NotFoundError ex)
        {
            return NotFound(ex.Payload);
        }
        catch (AppError ex) when (ex.HttpStatusCode == StatusCodes.Status400BadRequest)
        {
            var modelState = new ModelStateDictionary();
            modelState.AddModelError("idHabitCategory", ex.Payload.Details ?? ex.Payload.Message);
            return ValidationProblem(modelState);
        }
        catch (ServerError ex)
        {
            return StatusCode(ex.HttpStatusCode, ex.Payload);
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                new ErrorResponse
                {
                    Code = 500,
                    Message = "An unexpected error occurred.",
                    Details = ex.Message,
                }
            );
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateHabitDisciplineRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            var created = await _habitDisciplineService.CreateDisciplineAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.IdHabitDiscipline }, created);
        }
        catch (AppError ex) when (ex.HttpStatusCode == StatusCodes.Status400BadRequest)
        {
            var modelState = new ModelStateDictionary();
            modelState.AddModelError("idHabitCategory", ex.Payload.Details ?? ex.Payload.Message);
            return ValidationProblem(modelState);
        }
        catch (ServerError ex)
        {
            return StatusCode(ex.HttpStatusCode, ex.Payload);
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                new ErrorResponse
                {
                    Code = 500,
                    Message = "An unexpected error occurred.",
                    Details = ex.Message,
                }
            );
        }
    }
}
