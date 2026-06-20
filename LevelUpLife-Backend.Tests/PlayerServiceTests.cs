using LevelUpLifeBackend.Infrastructure.Configuration;
using LevelUpLifeBackend.Models;
using LevelUpLifeBackend.Repositories;
using LevelUpLifeBackend.Services;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace LevelUpLifeBackend.Tests;

public class PlayerServiceTests
{
    private readonly Mock<IPlayerRepository> _playerRepositoryMock;
    private readonly PlayerService _playerService;

    public PlayerServiceTests()
    {
        _playerRepositoryMock = new Mock<IPlayerRepository>();
        var avatarStorageMock = new Mock<IAvatarStorageService>();
        _playerService = new PlayerService(
            _playerRepositoryMock.Object,
            new LevelProgressService(
                Options.Create(
                    new LevelingOptions
                    {
                        Strategy = LevelingStrategy.EscalatingPercent,
                        BaseXpPerLevel = 100,
                        EscalationPercent = 20,
                    })),
            avatarStorageMock.Object,
            Options.Create(new PlayerProfileOptions()));
    }

    [Fact]
    public async Task GetProfileAsync_WhenPlayerExists_ReturnsMappedProfile()
    {
        var player = CreatePlayer(lastLogin: new DateTime(2026, 5, 10, 14, 30, 0, DateTimeKind.Utc));
        _playerRepositoryMock
            .Setup(repo => repo.GetActiveByIdWithRelationsAsync(1))
            .ReturnsAsync(player);

        var result = await _playerService.GetProfileAsync(1);

        Assert.Equal(GetPlayerProfileStatus.Success, result.Status);
        Assert.NotNull(result.Response);
        Assert.Equal("1", result.Response.PlayerUserId);
        Assert.Equal("testuser", result.Response.PlayerUserUserName);
        Assert.Equal(5, result.Response.PlayerUserLevel);
        Assert.True(result.Response.StatusIsActive);
        Assert.Equal(new DateTime(2026, 5, 10, 14, 30, 0, DateTimeKind.Utc), result.Response.PlayerUserLastLogin);
        Assert.Equal("Test", result.Response.PersonData.Name);
        Assert.Equal("User", result.Response.PersonData.LastName);
        Assert.Equal("test@leveluplife.com", result.Response.PersonData.Email);
        Assert.Equal(new DateOnly(1990, 1, 1), result.Response.PersonData.Birthdate);
        Assert.Equal(300, result.Response.Gold);
        Assert.False(string.IsNullOrWhiteSpace(result.ETag));
    }

    [Fact]
    public async Task GetProfileAsync_WhenLastLoginIsNull_ReturnsNullLastLogin()
    {
        var player = CreatePlayer(lastLogin: null);
        _playerRepositoryMock
            .Setup(repo => repo.GetActiveByIdWithRelationsAsync(1))
            .ReturnsAsync(player);

        var result = await _playerService.GetProfileAsync(1);

        Assert.Equal(GetPlayerProfileStatus.Success, result.Status);
        Assert.NotNull(result.Response);
        Assert.Null(result.Response.PlayerUserLastLogin);
    }

    [Fact]
    public async Task GetProfileAsync_WhenLastLoginIsDefaultDateTime_ReturnsNullLastLogin()
    {
        var player = CreatePlayer(lastLogin: default);
        _playerRepositoryMock
            .Setup(repo => repo.GetActiveByIdWithRelationsAsync(1))
            .ReturnsAsync(player);

        var result = await _playerService.GetProfileAsync(1);

        Assert.Equal(GetPlayerProfileStatus.Success, result.Status);
        Assert.NotNull(result.Response);
        Assert.Null(result.Response.PlayerUserLastLogin);
    }

    [Fact]
    public async Task GetProfileAsync_WhenPlayerNotFound_ReturnsNotFound()
    {
        _playerRepositoryMock
            .Setup(repo => repo.GetActiveByIdWithRelationsAsync(99))
            .ReturnsAsync((PlayerUser?)null);

        var result = await _playerService.GetProfileAsync(99);

        Assert.Equal(GetPlayerProfileStatus.NotFound, result.Status);
        Assert.Null(result.Response);
        Assert.Null(result.ETag);
    }

    private static PlayerUser CreatePlayer(DateTime? lastLogin)
    {
        return new PlayerUser
        {
            Id = 1,
            UserName = "testuser",
            Level = 5,
            ExperiencePoints = 464,
            Gold = 300,
            CreationDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            IsActive = true,
            LastLogin = lastLogin,
            Person = new Person
            {
                Id = 10,
                Name = "Test",
                LastName = "User",
                Email = "test@leveluplife.com",
                BirthDate = new DateOnly(1990, 1, 1),
                IsActive = true
            },
            Class = new UserPlayerClass
            {
                Id = 2,
                Name = "Warrior",
                IsActive = true
            }
        };
    }
}
