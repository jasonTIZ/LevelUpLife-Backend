using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.DTOs.Responses;

namespace LevelUpLifeBackend.Services;

// Orchestrates building the prompt, calling the LLM, and parsing the response.
public interface IAiChatService
{
    Task<AiChatResponseDto> ChatAsync(AiChatRequestDto request, CancellationToken ct = default);
}
