using LevelUpLifeBackend.Models;
using LevelUpLifeBackend.Mappers;
using Xunit;

namespace LevelUpLife_Backend.Tests;

public class PlayerProgressMapperTests
{
    [Theory]
    [InlineData(50, 50)]
    [InlineData(0, 10)]
    public void CalculateXpEarned_UsesTaskValueOrDifficultyFallback(int xpValue, int expectedXp)
    {
        var task = new HabitTask
        {
            XpValue = xpValue,
            Difficulty = TaskDifficulty.EASY,
        };

        Assert.Equal(expectedXp, PlayerProgressMapper.CalculateXpEarned(task));
    }
}
