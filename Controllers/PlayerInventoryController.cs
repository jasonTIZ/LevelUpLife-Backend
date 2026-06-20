using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LevelUpLifeBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LevelUpLifeBackend.Controllers;

[ApiController]
[Route("api/Player/inventory")]
[Authorize]
public class PlayerInventoryController : ControllerBase
{
    private readonly IPlayerInventoryService _playerInventoryService;

    public PlayerInventoryController(IPlayerInventoryService playerInventoryService)
    {
        _playerInventoryService = playerInventoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetInventory()
    {
        var userId = ResolveAuthenticatedUserId();
        if (userId is null)
        {
            return Unauthorized(new
            {
                code = 401,
                message = "Unauthorized",
                details = "Token inválido o sin identificador de usuario.",
            });
        }

        var items = await _playerInventoryService.GetByPlayerIdAsync(userId.Value);
        return Ok(items);
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
