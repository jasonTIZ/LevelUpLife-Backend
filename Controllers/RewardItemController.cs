using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
    private readonly IPlayerInventoryService _inventoryService;

    public RewardItemController(
        IRewardItemService rewardItemService,
        IPlayerInventoryService inventoryService)
    {
        _rewardItemService = rewardItemService;
        _inventoryService = inventoryService;
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

    [HttpPost("{itemId:int}/purchase")]
    public async Task<IActionResult> Purchase(int itemId)
    {
        var userId = ResolveUserId();
        if (userId is null)
        {
            return Unauthorized(new
            {
                code = "UNAUTHORIZED",
                message = "Unauthorized access.",
                details = "A valid JWT or X-User-Id header is required.",
            });
        }

        try
        {
            var result = await _inventoryService.PurchaseAsync(userId.Value, itemId);
            return StatusCode(StatusCodes.Status201Created, result);
        }
        catch (NotFoundError ex)
        {
            return NotFound(ex.Payload);
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

    [HttpGet("inventory")]
    public async Task<IActionResult> GetInventory()
    {
        var userId = ResolveUserId();
        if (userId is null)
        {
            return Unauthorized(new
            {
                code = "UNAUTHORIZED",
                message = "Unauthorized access.",
                details = "A valid JWT or X-User-Id header is required.",
            });
        }

        try
        {
            var items = await _inventoryService.GetInventoryAsync(userId.Value);
            return Ok(items);
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

    private int? ResolveUserId()
    {
        var headerValue = Request.Headers["X-User-Id"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(headerValue) && int.TryParse(headerValue, out var headerUserId))
            return headerUserId;

        var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (!string.IsNullOrWhiteSpace(claimValue) && int.TryParse(claimValue, out var claimUserId))
            return claimUserId;

        return null;
    }
}
