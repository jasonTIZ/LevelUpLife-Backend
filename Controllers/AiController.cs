using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace LevelUpLifeBackend.Controllers;

[ApiController]
[Route("api/ai")]
public class AiController : ControllerBase
{
    private readonly IAiChatService _aiChatService;

    public AiController(IAiChatService aiChatService)
    {
        _aiChatService = aiChatService;
    }

    // POST /api/ai/chat
    // Receives the model, system prompt, username, and conversation history,
    // forwards them to the local LLM gateway, and returns a structured reply.
    [HttpPost("chat")]
    public async Task<IActionResult> Chat(
        [FromBody] AiChatRequestDto request,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var response = await _aiChatService.ChatAsync(request, ct);
            return Ok(response);
        }
        catch (HttpRequestException ex)
        {
            // The LLM gateway is down or unreachable — return a hardcoded fallback so the app keeps working.
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    code = "AI_PROVIDER_ERROR",
                    message = "The AI service is currently unavailable.",
                    details = ex.Message,
                    fallback = new
                    {
                        reply = "I'm temporarily unavailable, but keep up with your habits — you're doing great!",
                        suggestions = new[]
                        {
                            "Review your active habits and log today's progress.",
                            "Pick one small action that moves you closer to your goal.",
                        },
                        recommendations = new[]
                        {
                            "Stay consistent — daily small wins add up faster than you think.",
                            "Come back later and I'll be ready to help you plan your next steps.",
                        },
                    },
                }
            );
        }
        catch (OperationCanceledException)
        {
            // Request was cancelled, usually because the LLM took too long to respond.
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                code = "REQUEST_TIMEOUT",
                message = "The AI service took too long to respond.",
                details = "Please try again.",
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                code = "SERVER_ERROR",
                message = "An unexpected error occurred.",
                details = ex.Message,
            });
        }
    }
}
