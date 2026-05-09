using System.Text.Json.Serialization;

namespace LevelUpLifeBackend.Infrastructure.Errors;

/// <summary>
/// Cuerpo de error estándar devuelto por APIs externas (issue #46).
/// </summary>
public sealed class ErrorResponse
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("details")]
    public string Details { get; set; } = string.Empty;
}
