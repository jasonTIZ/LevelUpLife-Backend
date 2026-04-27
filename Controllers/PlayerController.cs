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

    [HttpPut("update")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdatePlayerProfileRequestDto request)
    {
        var userIdValue =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (string.IsNullOrWhiteSpace(userIdValue) || !int.TryParse(userIdValue, out int playerUserId))
        {
            return Unauthorized(new
            {
                code = 401,
                message = "Unauthorized",
                details = "Token inválido o sin identificador de usuario."
            });
        }

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

    private IActionResult BuildSuccessResponse(UpdatePlayerProfileServiceResult result)
    {
        if (!string.IsNullOrWhiteSpace(result.ETag))
        {
            Response.Headers.ETag = result.ETag;
        }

        return Ok(result.Response);
    }
}
