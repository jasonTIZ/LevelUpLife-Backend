using LevelUpLifeBackend.Models;
using LevelUpLifeBackend.Repositories;
using LevelUpLifeBackend.Services;
using Moq;
using Xunit;

namespace LevelUpLifeBackend.Tests;

public class PlayerInventoryServiceTests
{
    private readonly Mock<IPlayerInventoryRepository> _repositoryMock;
    private readonly PlayerInventoryService _service;

    public PlayerInventoryServiceTests()
    {
        _repositoryMock = new Mock<IPlayerInventoryRepository>();
        _service = new PlayerInventoryService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetByPlayerIdAsync_WhenInventoryEmpty_ReturnsEmptyList()
    {
        _repositoryMock
            .Setup(repo => repo.GetByPlayerIdAsync(1))
            .ReturnsAsync(Array.Empty<PlayerInventory>());

        var result = await _service.GetByPlayerIdAsync(1);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByPlayerIdAsync_WhenInventoryHasItem_ReturnsMappedDto()
    {
        var acquiredAt = new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc);
        _repositoryMock
            .Setup(repo => repo.GetByPlayerIdAsync(1))
            .ReturnsAsync(new[]
            {
                new PlayerInventory
                {
                    Id = 10,
                    PlayerUserId = 1,
                    RewardItemId = 2,
                    Quantity = 3,
                    IsEquipped = true,
                    AcquiredAt = acquiredAt,
                    RewardItem = new RewardItem
                    {
                        Id = 2,
                        TypeId = 1,
                        Name = "Poción Protectora",
                        Description = "Protege la racha",
                        CostGold = 500,
                        EffectValue = null,
                        IsActive = true,
                        Type = new RewardItemType
                        {
                            Id = 1,
                            Name = "Poción",
                            IsActive = true,
                        },
                    },
                },
            });

        var result = (await _service.GetByPlayerIdAsync(1)).ToList();

        Assert.Single(result);
        Assert.Equal(10, result[0].Id);
        Assert.Equal(2, result[0].RewardItemId);
        Assert.Equal("Poción Protectora", result[0].ItemName);
        Assert.Equal("Poción", result[0].TypeName);
        Assert.Equal(3, result[0].Quantity);
        Assert.True(result[0].IsEquipped);
        Assert.Equal(acquiredAt, result[0].AcquiredAt);
        Assert.Equal(500, result[0].CostGold);
    }
}
