namespace LevelUpLifeBackend.Infrastructure.Ai;

// Configuration for the LLM gateway, bound from the "Ai" section in appsettings.
public sealed class AiOptions
{
    public const string SectionName = "Ai";

    // Secret key sent in the x-api-key header to authenticate with the gateway.
    public string ApiKey { get; set; } = string.Empty;

    // Base URL of the gateway (e.g. https://chat.floridafoodlicense.com).
    public string BaseUrl { get; set; } = string.Empty;
}
