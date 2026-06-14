using LevelUpLifeBackend.Infrastructure;
using LevelUpLifeBackend.Models;
using Xunit;

namespace LevelUpLife_Backend.Tests;

public class StreakProtectionTypeParserTests
{
    [Theory]
    [InlineData("TRABAJO", StreakProtectionType.TRABAJO)]
    [InlineData("evaluacion", StreakProtectionType.EVALUACION)]
    [InlineData(" EMERGENCIA ", StreakProtectionType.EMERGENCIA)]
    public void TryParse_WhenValueIsValid_ReturnsTrue(string value, StreakProtectionType expected)
    {
        var parsed = StreakProtectionTypeParser.TryParse(value, out var protectionType);

        Assert.True(parsed);
        Assert.Equal(expected, protectionType);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("INVALID")]
    [InlineData("123")]
    public void TryParse_WhenValueIsInvalid_ReturnsFalse(string? value)
    {
        var parsed = StreakProtectionTypeParser.TryParse(value, out _);

        Assert.False(parsed);
    }
}
