using LevelUpLifeBackend.Infrastructure.Configuration;
using LevelUpLifeBackend.Services;
using Microsoft.Extensions.Options;
using Xunit;

namespace LevelUpLife_Backend.Tests;

public class LevelProgressServiceTests
{
    private static LevelProgressService CreateService(
        int baseXp = 100,
        int escalationPercent = 20)
    {
        return new LevelProgressService(
            Options.Create(
                new LevelingOptions
                {
                    Strategy = LevelingStrategy.EscalatingPercent,
                    BaseXpPerLevel = baseXp,
                    EscalationPercent = escalationPercent,
                }));
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(99, 1)]
    [InlineData(100, 2)]
    [InlineData(199, 2)]
    [InlineData(200, 3)]
    public void CalculateLevel_MapsTotalXpToProgressiveLevel(int totalXp, int expectedLevel)
    {
        var service = CreateService();

        Assert.Equal(expectedLevel, service.CalculateLevel(totalXp));
    }

    [Fact]
    public void GetXpRequiredForLevel_UsesSameBaseForLevelsOneAndTwo()
    {
        var service = CreateService();

        Assert.Equal(100, service.GetXpRequiredForLevel(1));
        Assert.Equal(100, service.GetXpRequiredForLevel(2));
    }

    [Fact]
    public void GetXpRequiredForLevel_EscalatesTwentyPercentFromLevelThree()
    {
        var service = CreateService();

        Assert.Equal(120, service.GetXpRequiredForLevel(3));
        Assert.Equal(144, service.GetXpRequiredForLevel(4));
    }

    [Fact]
    public void GetTotalXpForLevel_MatchesProgressiveThresholdTable()
    {
        var service = CreateService();

        Assert.Equal(0, service.GetTotalXpForLevel(1));
        Assert.Equal(100, service.GetTotalXpForLevel(2));
        Assert.Equal(200, service.GetTotalXpForLevel(3));
        Assert.Equal(320, service.GetTotalXpForLevel(4));
    }

    [Fact]
    public void GetLevelProgress_WhenMidTier_ReturnsPartialProgress()
    {
        var service = CreateService();
        var progress = service.GetLevelProgress(145);

        Assert.Equal(2, progress.CurrentLevel);
        Assert.Equal(45, progress.ExperiencePointsInCurrentLevel);
        Assert.Equal(100, progress.ExperiencePointsRequiredForNextLevel);
        Assert.Equal(0.45, progress.LevelProgressPercent);
    }

    [Fact]
    public void GetLevelProgress_WhenExactlyAtLevelUpBoundary_ResetsProgressInNewTier()
    {
        var service = CreateService();
        var progress = service.GetLevelProgress(200);

        Assert.Equal(3, progress.CurrentLevel);
        Assert.Equal(0, progress.ExperiencePointsInCurrentLevel);
        Assert.Equal(120, progress.ExperiencePointsRequiredForNextLevel);
        Assert.Equal(0.0, progress.LevelProgressPercent);
    }

    [Fact]
    public void ToCompleteResponse_WhenNoLevelUp_ReturnsAccurateProgressFields()
    {
        var service = CreateService();
        var response = service.ToCompleteResponse(
            xpEarned: 10,
            previousLevel: 1,
            totalExperiencePoints: 10,
            streakUpdated: true,
            daysStreak: 2);

        Assert.Equal(10, response.XpEarned);
        Assert.Equal(1, response.PreviousLevel);
        Assert.Equal(1, response.NewLevel);
        Assert.False(response.LeveledUp);
        Assert.Equal(10, response.ExperiencePointsInCurrentLevel);
        Assert.Equal(100, response.ExperiencePointsRequiredForNextLevel);
        Assert.Equal(0.1, response.LevelProgressPercent);
        Assert.True(response.StreakUpdated);
        Assert.Equal(2, response.DaysStreak);
        Assert.Equal("escalating_percent", response.LevelingConfig.Strategy);
    }

    [Fact]
    public void ToCompleteResponse_WhenLevelUp_ReturnsPreviousAndNewLevel()
    {
        var service = CreateService();
        var response = service.ToCompleteResponse(
            xpEarned: 100,
            previousLevel: 1,
            totalExperiencePoints: 150,
            streakUpdated: true,
            daysStreak: 1);

        Assert.Equal(1, response.PreviousLevel);
        Assert.Equal(2, response.NewLevel);
        Assert.True(response.LeveledUp);
        Assert.Equal(50, response.ExperiencePointsInCurrentLevel);
        Assert.Equal(100, response.ExperiencePointsRequiredForNextLevel);
        Assert.Equal(0.5, response.LevelProgressPercent);
    }
}
