using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.Infrastructure.Errors;
using LevelUpLifeBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace LevelUpLifeBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RewardItemController : ControllerBase
{
    private readonly IRewardItemService _rewardItemService;

    public RewardItemController(IRewardItemService rewardItemService)
    {
        _rewardItemService = rewardItemService;
    }

    [HttpGet]
    public async Task<IActionResult> GetRewardItems([FromBody] RewardItemFilterRequestDto? filter = null)
    {
        try
        {
            var items = await _rewardItemService.GetFilteredAsync(filter);
            return Ok(items);
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
