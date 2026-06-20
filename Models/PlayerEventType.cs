namespace LevelUpLifeBackend.Models;

public enum PlayerEventType
{
    STREAK_RESET,
    STREAK_CONTINUED,
    STREAK_PROTECTION_USED,
    LEVEL_UP,
    XP_PENALTY,
    XP_PENALTY_REVERTED,
}
