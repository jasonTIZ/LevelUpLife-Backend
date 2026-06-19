using LevelUpLifeBackend.DTOs.Responses;
using LevelUpLifeBackend.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace LevelUpLifeBackend.Services;

public class LevelProgressService : ILevelProgressService
{
    private readonly LevelingOptions _options;

    public LevelProgressService(IOptions<LevelingOptions> options)
    {
        _options = options.Value;
    }

    public int GetXpRequiredForLevel(int level)
    {
        EnsureSupportedStrategy();

        if (level < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(level), "Level must be at least 1.");
        }

        if (level == 1)
        {
            return _options.BaseXpPerLevel;
        }

        var escalationFactor = 1.0 + (_options.EscalationPercent / 100.0);
        return (int)Math.Round(_options.BaseXpPerLevel * Math.Pow(escalationFactor, level - 2));
    }

    public int GetTotalXpForLevel(int level)
    {
        if (level <= 1)
        {
            return 0;
        }

        var total = 0;
        for (var currentLevel = 1; currentLevel < level; currentLevel++)
        {
            total += GetXpRequiredForLevel(currentLevel);
        }

        return total;
    }

    public int CalculateLevel(int totalExperiencePoints)
    {
        var totalXp = Math.Max(0, totalExperiencePoints);
        var level = 1;

        while (GetTotalXpForLevel(level + 1) <= totalXp)
        {
            level++;
        }

        return level;
    }

    public LevelProgressDto GetLevelProgress(int totalExperiencePoints)
    {
        var totalXp = Math.Max(0, totalExperiencePoints);
        var currentLevel = CalculateLevel(totalXp);
        var xpAtLevelStart = GetTotalXpForLevel(currentLevel);
        var xpInCurrentLevel = totalXp - xpAtLevelStart;
        var xpRequiredForNextLevel = GetXpRequiredForLevel(currentLevel);
        var progressPercent = xpRequiredForNextLevel > 0
            ? Math.Min(1.0, (double)xpInCurrentLevel / xpRequiredForNextLevel)
            : 1.0;

        return new LevelProgressDto
        {
            CurrentLevel = currentLevel,
            TotalExperiencePoints = totalXp,
            ExperiencePointsInCurrentLevel = xpInCurrentLevel,
            ExperiencePointsRequiredForNextLevel = xpRequiredForNextLevel,
            LevelProgressPercent = Math.Round(progressPercent, 4),
        };
    }

    public LevelingConfigDto GetLevelingConfig()
    {
        return new LevelingConfigDto
        {
            Strategy = MapStrategyToApiValue(_options.Strategy),
            BaseXpPerLevel = _options.BaseXpPerLevel,
            EscalationPercent = _options.EscalationPercent,
        };
    }

    public CompleteHabitTaskResponseDto ToCompleteResponse(
        int xpEarned,
        int previousLevel,
        int totalExperiencePoints,
        bool streakUpdated,
        int daysStreak)
    {
        var progress = GetLevelProgress(totalExperiencePoints);
        var newLevel = progress.CurrentLevel;

        return new CompleteHabitTaskResponseDto
        {
            XpEarned = xpEarned,
            PreviousLevel = previousLevel,
            NewLevel = newLevel,
            TotalExperiencePoints = progress.TotalExperiencePoints,
            ExperiencePointsInCurrentLevel = progress.ExperiencePointsInCurrentLevel,
            ExperiencePointsRequiredForNextLevel = progress.ExperiencePointsRequiredForNextLevel,
            LevelProgressPercent = progress.LevelProgressPercent,
            LeveledUp = newLevel > previousLevel,
            StreakUpdated = streakUpdated,
            DaysStreak = daysStreak,
            LevelingConfig = GetLevelingConfig(),
        };
    }

    private void EnsureSupportedStrategy()
    {
        if (_options.Strategy != LevelingStrategy.EscalatingPercent)
        {
            throw new NotSupportedException(
                $"Leveling strategy '{_options.Strategy}' is not implemented yet."
            );
        }
    }

    private static string MapStrategyToApiValue(LevelingStrategy strategy) =>
        strategy switch
        {
            LevelingStrategy.EscalatingPercent => "escalating_percent",
            _ => strategy.ToString().ToLowerInvariant(),
        };
}
