namespace LevelUpLifeBackend.Infrastructure.Ai;

// Contract for the LLM gateway. Returns the reply text and the session ID assigned by the gateway.
public interface IAiProvider
{
    Task<(string Reply, string? SessionId)> CompleteAsync(
        string userId,
        string message,
        string? systemPrompt,
        string? model,
        CancellationToken ct = default);
}
