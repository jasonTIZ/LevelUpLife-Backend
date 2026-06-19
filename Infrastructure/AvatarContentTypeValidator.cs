namespace LevelUpLifeBackend.Infrastructure;

public static class AvatarContentTypeValidator
{
    private static readonly Dictionary<string, string[]> Signatures = new(StringComparer.OrdinalIgnoreCase)
    {
        ["image/jpeg"] = ["FF D8 FF"],
        ["image/png"] = ["89 50 4E 47 0D 0A 1A 0A"],
        ["image/webp"] = ["52 49 46 46"],
    };

    public static bool IsAllowedContentType(string? contentType, IEnumerable<string> allowedContentTypes)
    {
        if (string.IsNullOrWhiteSpace(contentType))
        {
            return false;
        }

        return allowedContentTypes.Contains(contentType.Trim(), StringComparer.OrdinalIgnoreCase);
    }

    public static string ResolveExtension(string contentType) =>
        contentType.ToLowerInvariant() switch
        {
            "image/jpeg" => ".jpg",
            "image/png" => ".png",
            "image/webp" => ".webp",
            _ => string.Empty,
        };

    public static bool HasValidSignature(Stream stream, string contentType)
    {
        if (!Signatures.TryGetValue(contentType, out var expectedSignatures))
        {
            return false;
        }

        var originalPosition = stream.CanSeek ? stream.Position : 0;
        Span<byte> header = stackalloc byte[12];
        var read = stream.Read(header);

        if (stream.CanSeek)
        {
            stream.Position = originalPosition;
        }

        if (read <= 0)
        {
            return false;
        }

        var headerHex = Convert.ToHexString(header[..read]);
        return expectedSignatures.Any(signature =>
            headerHex.StartsWith(signature.Replace(" ", string.Empty), StringComparison.OrdinalIgnoreCase));
    }
}
