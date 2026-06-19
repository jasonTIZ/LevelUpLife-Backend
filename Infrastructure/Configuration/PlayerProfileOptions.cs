namespace LevelUpLifeBackend.Infrastructure.Configuration;

public class PlayerProfileOptions
{
    public const string SectionName = "PlayerProfile";

    public int MaxBioLength { get; set; } = 500;

    public long MaxAvatarBytes { get; set; } = 5 * 1024 * 1024;

    public string[] AllowedAvatarContentTypes { get; set; } =
    [
        "image/jpeg",
        "image/png",
        "image/webp",
    ];

    public string AvatarStoragePath { get; set; } = "uploads/avatars";

    public string AvatarPublicBasePath { get; set; } = "/uploads/avatars";
}
