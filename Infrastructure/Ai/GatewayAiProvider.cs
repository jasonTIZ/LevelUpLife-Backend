using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LevelUpLifeBackend.Infrastructure.Ai;

// Calls the custom LLM gateway (POST /chat).
// The gateway manages conversation history server-side using user_id,
// so we only send the latest message — not the full history.
public sealed class GatewayAiProvider : IAiProvider
{
    private readonly HttpClient _httpClient;

    // Omit null fields so the gateway uses its own defaults (e.g. default model).
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public GatewayAiProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<(string Reply, string? SessionId)> CompleteAsync(
        string userId,
        string message,
        string? systemPrompt,
        string? model,
        CancellationToken ct = default)
    {
        var body = new GatewayRequest
        {
            UserId = userId,
            Message = message,
            SystemPrompt = systemPrompt,
            Model = model
        };

        using var content = new StringContent(
            JsonSerializer.Serialize(body, SerializerOptions),
            Encoding.UTF8,
            "application/json"
        );

        // Gateway endpoint: POST /chat
        using var response = await _httpClient.PostAsync("chat", content, ct);
        response.EnsureSuccessStatusCode();

        // Parse { "reply": "...", "session_id": "..." }
        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync(ct));
        var root = doc.RootElement;

        var reply = root.GetProperty("reply").GetString() ?? string.Empty;
        var sessionId = root.TryGetProperty("session_id", out var sid) ? sid.GetString() : null;

        return (reply, sessionId);
    }

    // Internal model matching the gateway's expected JSON body.
    private sealed class GatewayRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? SystemPrompt { get; set; }
        public string? Model { get; set; }
    }
}
