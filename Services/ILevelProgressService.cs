using LevelUpLifeBackend.DTOs.Responses;

namespace LevelUpLifeBackend.Services;

public interface ILevelProgressService
{
    int GetXpRequiredForLevel(int level);

    int GetTotalXpForLevel(int level);

    int CalculateLevel(int totalExperiencePoints);

    LevelProgressDto GetLevelProgress(int totalExperiencePoints);

    LevelingConfigDto GetLevelingConfig();

    CompleteHabitTaskResponseDto ToCompleteResponse(
        int xpEarned,
        int previousLevel,
        int totalExperiencePoints,
        bool streakUpdated,
        int daysStreak);
}
