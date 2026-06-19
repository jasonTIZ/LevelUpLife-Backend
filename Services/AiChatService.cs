using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.DTOs.Responses;
using LevelUpLifeBackend.Infrastructure.Ai;

namespace LevelUpLifeBackend.Services;

public class AiChatService : IAiChatService
{
    private readonly IAiProvider _aiProvider;

    public AiChatService(IAiProvider aiProvider)
    {
        _aiProvider = aiProvider;
    }

    public async Task<AiChatResponseDto> ChatAsync(AiChatRequestDto request, CancellationToken ct = default)
    {
        // Forward the request to the gateway. Conversation history is managed server-side by user_id.
        var (reply, sessionId) = await _aiProvider.CompleteAsync(
            userId: request.Username,
            message: request.Message,
            systemPrompt: request.SystemPrompt,
            model: request.Model,
            ct: ct
        );

        return new AiChatResponseDto
        {
            Reply = reply,
            SessionId = sessionId
        };
    }
}
