namespace LevelUpLifeBackend.Infrastructure.Configuration;

public class StreakProtectionOptions
{
    public const string SectionName = "StreakProtection";

    public int MaxPerMonth { get; set; } = 3;
}
