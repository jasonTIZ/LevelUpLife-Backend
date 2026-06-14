using LevelUpLifeBackend.Models;
using LevelUpLifeBackend.Services;
using Xunit;

namespace LevelUpLife_Backend.Tests;

public class StreakCalculatorTests
{
    [Fact]
    public void ComputeStreakCount_WhenNoPreviousLog_ReturnsOne()
    {
        var (streak, wasReset) = StreakCalculator.ComputeStreakCount(null, new DateOnly(2026, 5, 10), false);

        Assert.Equal(1, streak);
        Assert.False(wasReset);
    }

    [Fact]
    public void ComputeStreakCount_WhenConsecutiveDay_IncrementsStreak()
    {
        var last = new StreakLog
        {
            LogDate = new DateOnly(2026, 5, 9),
            StreakCount = 4,
            CompletionRecorded = true,
        };

        var (streak, wasReset) = StreakCalculator.ComputeStreakCount(
            last,
            new DateOnly(2026, 5, 10),
            false
        );

        Assert.Equal(5, streak);
        Assert.False(wasReset);
    }

    [Fact]
    public void ComputeStreakCount_WhenGapWithoutProtection_ResetsToOne()
    {
        var last = new StreakLog
        {
            LogDate = new DateOnly(2026, 5, 5),
            StreakCount = 10,
            CompletionRecorded = true,
        };

        var (streak, wasReset) = StreakCalculator.ComputeStreakCount(
            last,
            new DateOnly(2026, 5, 10),
            gapHasProtection: false
        );

        Assert.Equal(1, streak);
        Assert.True(wasReset);
    }

    [Fact]
    public void ComputeStreakCount_WhenGapWithProtection_ContinuesStreak()
    {
        var last = new StreakLog
        {
            LogDate = new DateOnly(2026, 5, 5),
            StreakCount = 10,
            CompletionRecorded = true,
        };

        var (streak, wasReset) = StreakCalculator.ComputeStreakCount(
            last,
            new DateOnly(2026, 5, 10),
            gapHasProtection: true
        );

        Assert.Equal(11, streak);
        Assert.False(wasReset);
    }

    [Fact]
    public void ComputeStreakCount_WhenFourDayGapWithSingleProtection_ContinuesStreakAsSalvavidas()
    {
        var last = new StreakLog
        {
            LogDate = new DateOnly(2026, 5, 1),
            StreakCount = 7,
            CompletionRecorded = true,
        };

        // Hueco de 4 días; solo 1 día protegido en el intervalo → modelo salvavidas (+1, no reinicio).
        var (streak, wasReset) = StreakCalculator.ComputeStreakCount(
            last,
            new DateOnly(2026, 5, 5),
            gapHasProtection: true
        );

        Assert.Equal(8, streak);
        Assert.False(wasReset);
    }

    [Fact]
    public void ComputeStreakCount_WhenFourDayGapWithoutProtection_ResetsToOne()
    {
        var last = new StreakLog
        {
            LogDate = new DateOnly(2026, 5, 1),
            StreakCount = 7,
            CompletionRecorded = true,
        };

        var (streak, wasReset) = StreakCalculator.ComputeStreakCount(
            last,
            new DateOnly(2026, 5, 5),
            gapHasProtection: false
        );

        Assert.Equal(1, streak);
        Assert.True(wasReset);
    }
}
