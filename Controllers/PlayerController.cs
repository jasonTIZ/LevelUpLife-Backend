using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LevelUpLifeBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PlayerController : ControllerBase
{
    private readonly IPlayerService _playerService;

    public PlayerController(IPlayerService playerService)
    {
        _playerService = playerService;
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userIdResult = ResolveAuthenticatedUserId();
        if (userIdResult is null)
        {
            return Unauthorized(new
            {
                code = 401,
                message = "Unauthorized",
                details = "Token inválido o sin identificador de usuario."
            });
        }

        var result = await _playerService.GetProfileAsync(userIdResult.Value);

        return result.Status switch
        {
            GetPlayerProfileStatus.Success => BuildProfileSuccessResponse(result),
            GetPlayerProfileStatus.NotFound => NotFound(new
            {
                code = 404,
                message = "Not Found",
                details = "No se encontró el jugador solicitado."
            }),
            _ => StatusCode(StatusCodes.Status500InternalServerError, new
            {
                code = 500,
                message = "Internal Server Error",
                details = "Unexpected error occurred"
            })
        };
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdatePlayerProfileRequestDto request)
    {
        var userIdResult = ResolveAuthenticatedUserId();
        if (userIdResult is null)
        {
            return Unauthorized(new
            {
                code = 401,
                message = "Unauthorized",
                details = "Token inválido o sin identificador de usuario."
            });
        }
        var playerUserId = userIdResult.Value;

        if (!Request.Headers.TryGetValue("If-Match", out var ifMatchHeader) || string.IsNullOrWhiteSpace(ifMatchHeader))
        {
            return BadRequest(new
            {
                code = 400,
                message = "Bad Request",
                details = "El header If-Match es requerido."
            });
        }

        var result = await _playerService.UpdateProfileAsync(playerUserId, ifMatchHeader.ToString(), request);

        return result.Status switch
        {
            UpdatePlayerProfileStatus.Success => BuildSuccessResponse(result),
            UpdatePlayerProfileStatus.NotFound => NotFound(new
            {
                code = 404,
                message = "Not Found",
                details = "No se encontró el jugador solicitado."
            }),
            UpdatePlayerProfileStatus.UsernameTaken => Conflict(new
            {
                code = 409,
                message = "Conflict",
                details = "El username ya está en uso"
            }),
            UpdatePlayerProfileStatus.ETagMismatch => StatusCode(StatusCodes.Status412PreconditionFailed, new
            {
                code = 412,
                message = "Precondition Failed",
                details = "El recurso fue modificado por otra operación. Obtenga el ETag actualizado."
            }),
            UpdatePlayerProfileStatus.InvalidData => BadRequest(new
            {
                code = 400,
                message = "Bad Request",
                details = result.Details ?? "Datos inválidos."
            }),
            _ => StatusCode(StatusCodes.Status500InternalServerError, new
            {
                code = 500,
                message = "Internal Server Error",
                details = "Unexpected error occurred"
            })
        };
    }

    private IActionResult BuildProfileSuccessResponse(GetPlayerProfileServiceResult result)
    {
        if (!string.IsNullOrWhiteSpace(result.ETag))
        {
            Response.Headers.ETag = result.ETag;
        }

        return Ok(result.Response);
    }

    private IActionResult BuildSuccessResponse(UpdatePlayerProfileServiceResult result)
    {
        if (!string.IsNullOrWhiteSpace(result.ETag))
        {
            Response.Headers.ETag = result.ETag;
        }

        return Ok(result.Response);
    }

    [HttpPatch("delete")]
    public async Task<IActionResult> DeleteAccount([FromBody] DeletePlayerAccountRequestDto? request)
    {
        var userIdResult = ResolveAuthenticatedUserId();
        if (userIdResult is null)
        {
            return Unauthorized(new
            {
                code = 401,
                message = "Unauthorized",
                details = "Token inválido o sin identificador de usuario."
            });
        }

        var reasonFromHeader = Request.Headers["X-Delete-Reason"].FirstOrDefault();
        var reason = !string.IsNullOrWhiteSpace(request?.Reason) ? request!.Reason : reasonFromHeader;

        var result = await _playerService.DeleteAccountAsync(userIdResult.Value, reason);

        return result.Status switch
        {
            DeletePlayerAccountStatus.Success => Ok(result.Response),
            DeletePlayerAccountStatus.NotFound => NotFound(new
            {
                code = 404,
                message = "Not Found",
                details = "No se encontró el jugador solicitado."
            }),
            DeletePlayerAccountStatus.Forbidden => StatusCode(StatusCodes.Status403Forbidden, new
            {
                code = 403,
                message = "Forbidden",
                details = "No tiene permisos para desactivar esta cuenta"
            }),
            _ => StatusCode(StatusCodes.Status500InternalServerError, new
            {
                code = 500,
                message = "Internal Server Error",
                details = "Unexpected error occurred"
            })
        };
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
