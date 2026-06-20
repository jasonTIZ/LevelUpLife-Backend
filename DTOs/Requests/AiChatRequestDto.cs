using System.ComponentModel.DataAnnotations;

namespace LevelUpLifeBackend.DTOs.Requests;

// Request body for POST /api/ai/chat
public class AiChatRequestDto
{
    // Unique user identifier — the gateway uses this to track conversation history server-side.
    [Required(ErrorMessage = "username is required.")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "username must be between 1 and 50 characters.")]
    public string Username { get; set; } = string.Empty;

    // The user's current message.
    [Required(ErrorMessage = "message is required.")]
    [StringLength(4000, MinimumLength = 1, ErrorMessage = "message must be between 1 and 4000 characters.")]
    public string Message { get; set; } = string.Empty;

    // Optional — defines the bot's personality and context for this session.
    // If omitted, the gateway uses its default system prompt.
    [StringLength(4000, ErrorMessage = "systemPrompt cannot exceed 4000 characters.")]
    public string? SystemPrompt { get; set; }

    // Optional — LLM model to use (e.g. "gemma2:9b", "gemma4:31b").
    // If omitted, the gateway uses its default model.
    [StringLength(100, ErrorMessage = "model cannot exceed 100 characters.")]
    public string? Model { get; set; }
}
