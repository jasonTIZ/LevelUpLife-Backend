using System.Security.Claims;
using LevelUpLifeBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LevelUpLifeBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HabitCategoryController : ControllerBase
{
    private readonly IHabitCategoryService _habitCategoryService;

    public HabitCategoryController(IHabitCategoryService habitCategoryService)
    {
        _habitCategoryService = habitCategoryService;
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetActiveHabitCategories(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null
    )
    {
        try
        {
            if (pageNumber < 1)
                pageNumber = 1;
            if (pageSize < 1 || pageSize > 100)
                pageSize = 10;

            // El conteo de hábitos por categoría se calcula solo para el usuario
            // autenticado (header X-User-Id). Si no viene, el conteo será 0.
            int? userId = null;
            var userIdString = Request.Headers["X-User-Id"].FirstOrDefault();
            if (int.TryParse(userIdString, out var parsedUserId))
                userId = parsedUserId;

            var pagedResult = await _habitCategoryService.GetActiveHabitCategoriesPaginatedAsync(
                pageNumber,
                pageSize,
                search,
                userId
            );

            if (pagedResult.Items == null || !pagedResult.Items.Any())
            {
                return BadRequest(
                    new { success = false, message = "No se han encontrado categorías de hábitos." }
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
