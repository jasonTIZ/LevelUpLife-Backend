using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.Infrastructure;

public static class StreakProtectionTypeParser
{
    public static bool TryParse(string? value, out StreakProtectionType protectionType)
    {
        protectionType = default;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        if (!Enum.TryParse(value.Trim(), ignoreCase: true, out protectionType))
        {
            return false;
        }

        return Enum.IsDefined(protectionType);
    }
}
