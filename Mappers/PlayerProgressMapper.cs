using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.Mappers;

public static class PlayerProgressMapper
{
    public static int CalculateXpEarned(HabitTask task)
    {
        if (task.XpValue > 0)
        {
            return task.XpValue;
        }

        return task.Difficulty switch
        {
            TaskDifficulty.EASY => 10,
            TaskDifficulty.MEDIUM => 25,
            TaskDifficulty.HARD => 50,
            _ => 10,
        };
    }

    public static StreakLog ToStreakLog(PlayerUser player, DateOnly logDate)
    {
        return new StreakLog
        {
            PlayerUserId = player.Id,
            StreakCount = player.DaysStreak,
            LogDate = logDate,
            ProtectionUsed = false,
            CompletionRecorded = true,
        };
    }

    public static StreakLog ToProtectionStreakLog(
        PlayerUser player,
        DateOnly logDate,
        StreakProtectionType protectionType
    )
    {
        return new StreakLog
        {
            PlayerUserId = player.Id,
            StreakCount = Math.Max(1, player.DaysStreak),
            LogDate = logDate,
            ProtectionUsed = true,
            ProtectionType = protectionType,
            CompletionRecorded = false,
        };
    }
}
