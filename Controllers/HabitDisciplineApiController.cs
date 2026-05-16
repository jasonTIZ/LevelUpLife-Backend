using LevelUpLifeBackend.Infrastructure.Errors;
using LevelUpLifeBackend.Services;
using Microsoft.AspNetCore.Mvc;

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
}
