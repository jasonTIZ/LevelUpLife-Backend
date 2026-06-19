namespace LevelUpLifeBackend.DTOs.Responses;

// Response body returned by POST /api/ai/chat
public class AiChatResponseDto
{
    // The LLM's reply to the user's message.
    public string Reply { get; set; } = string.Empty;

    // Session identifier assigned by the gateway — can be used for tracking or debugging.
    public string? SessionId { get; set; }
}
