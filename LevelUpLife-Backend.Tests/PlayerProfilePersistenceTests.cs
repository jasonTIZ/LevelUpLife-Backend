using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.Infrastructure.Configuration;
using LevelUpLifeBackend.Models;
using LevelUpLifeBackend.Repositories;
using LevelUpLifeBackend.Services;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace LevelUpLifeBackend.Tests;

public class PlayerProfilePersistenceTests
{
    private readonly Mock<IPlayerRepository> _playerRepositoryMock;
    private readonly Mock<IAvatarStorageService> _avatarStorageMock;
    private readonly PlayerService _playerService;

    public PlayerProfilePersistenceTests()
    {
        _playerRepositoryMock = new Mock<IPlayerRepository>();
        _avatarStorageMock = new Mock<IAvatarStorageService>();
        _avatarStorageMock
            .Setup(s => s.SaveAvatarAsync(It.IsAny<int>(), It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("/uploads/avatars/1/sample.png");
        _avatarStorageMock
            .Setup(s => s.DeleteAvatarIfExistsAsync(It.IsAny<string?>()))
            .Returns(Task.CompletedTask);

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
            _avatarStorageMock.Object,
            Options.Create(new PlayerProfileOptions()));
    }

    [Fact]
    public async Task GetProfileAsync_ReturnsBioAndAvatarUrl()
    {
        var player = CreatePlayer();
        player.Bio = "Habit builder";
        player.AvatarUrl = "/uploads/avatars/1/avatar.png";

        _playerRepositoryMock
            .Setup(repo => repo.GetActiveByIdWithRelationsAsync(1))
            .ReturnsAsync(player);

        var result = await _playerService.GetProfileAsync(1);

        Assert.Equal("Habit builder", result.Response!.Bio);
        Assert.Equal("/uploads/avatars/1/avatar.png", result.Response.AvatarUrl);
    }

    [Fact]
    public async Task UpdateProfileAsync_WhenBioIsValid_PersistsBio()
    {
        var player = CreatePlayer();
        _playerRepositoryMock
            .Setup(repo => repo.GetActiveByIdWithRelationsAsync(1))
            .ReturnsAsync(player);
        _playerRepositoryMock
            .Setup(repo => repo.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var etag = await GetCurrentETag(player);
        var result = await _playerService.UpdateProfileAsync(
            1,
            etag,
            new UpdatePlayerProfileRequestDto
            {
                PlayerData = new PlayerDataUpdateRequestDto
                {
                    Bio = "  New bio  ",
                },
            });

        Assert.Equal(UpdatePlayerProfileStatus.Success, result.Status);
        Assert.Equal("New bio", player.Bio);
        Assert.Equal("New bio", result.Response!.Player.Bio);
    }

    [Fact]
    public async Task UpdateProfileAsync_WhenBioIsTooLong_ReturnsInvalidData()
    {
        var player = CreatePlayer();
        _playerRepositoryMock
            .Setup(repo => repo.GetActiveByIdWithRelationsAsync(1))
            .ReturnsAsync(player);

        var etag = await GetCurrentETag(player);
        var result = await _playerService.UpdateProfileAsync(
            1,
            etag,
            new UpdatePlayerProfileRequestDto
            {
                PlayerData = new PlayerDataUpdateRequestDto
                {
                    Bio = new string('a', 501),
                },
            });

        Assert.Equal(UpdatePlayerProfileStatus.InvalidData, result.Status);
    }

    [Fact]
    public async Task UploadAvatarAsync_WhenImageIsValid_UpdatesAvatarUrl()
    {
        var player = CreatePlayer();
        _playerRepositoryMock
            .Setup(repo => repo.GetActiveByIdWithRelationsAsync(1))
            .ReturnsAsync(player);
        _playerRepositoryMock
            .Setup(repo => repo.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var etag = await GetCurrentETag(player);
        await using var pngStream = new MemoryStream(
            Convert.FromHexString("89504E470D0A1A0A0000000000000000"));

        var result = await _playerService.UploadAvatarAsync(
            1,
            etag,
            pngStream,
            "image/png",
            pngStream.Length);

        Assert.Equal(UploadPlayerAvatarStatus.Success, result.Status);
        Assert.Equal("/uploads/avatars/1/sample.png", player.AvatarUrl);
        Assert.Equal("/uploads/avatars/1/sample.png", result.Response!.Player.AvatarUrl);
    }

    [Fact]
    public async Task UploadAvatarAsync_WhenMimeTypeIsInvalid_ReturnsInvalidData()
    {
        var player = CreatePlayer();
        _playerRepositoryMock
            .Setup(repo => repo.GetActiveByIdWithRelationsAsync(1))
            .ReturnsAsync(player);

        var etag = await GetCurrentETag(player);
        await using var stream = new MemoryStream([0x00, 0x01, 0x02]);

        var result = await _playerService.UploadAvatarAsync(
            1,
            etag,
            stream,
            "application/pdf",
            stream.Length);

        Assert.Equal(UploadPlayerAvatarStatus.InvalidData, result.Status);
    }

    private async Task<string> GetCurrentETag(PlayerUser player)
    {
        _playerRepositoryMock
            .Setup(repo => repo.GetActiveByIdWithRelationsAsync(player.Id))
            .ReturnsAsync(player);

        var profile = await _playerService.GetProfileAsync(player.Id);
        return profile.ETag!;
    }

    private static PlayerUser CreatePlayer()
    {
        return new PlayerUser
        {
            Id = 1,
            UserName = "testuser",
            Level = 1,
            ExperiencePoints = 0,
            CreationDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            IsActive = true,
            Person = new Person
            {
                Id = 10,
                Name = "Test",
                LastName = "User",
                Email = "test@leveluplife.com",
                BirthDate = new DateOnly(1990, 1, 1),
                IsActive = true,
            },
            Class = new UserPlayerClass
            {
                Id = 2,
                Name = "Warrior",
                IsActive = true,
            },
        };
    }
}
