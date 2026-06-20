using LevelUpLifeBackend.Data;
using LevelUpLifeBackend.Infrastructure;
using LevelUpLifeBackend.Infrastructure.Configuration;
using LevelUpLifeBackend.Infrastructure.Errors;
using LevelUpLifeBackend.Models;
using LevelUpLifeBackend.Repositories;
using LevelUpLifeBackend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;
using Xunit;

namespace LevelUpLife_Backend.Tests;

public class PlayerInventoryServiceTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new AppDbContext(options);
    }

    private static ILevelProgressService CreateLevelProgressService() =>
        new LevelProgressService(
            Options.Create(
                new LevelingOptions
                {
                    Strategy = LevelingStrategy.EscalatingPercent,
                    BaseXpPerLevel = 100,
                    EscalationPercent = 20,
                }));

    private static async Task<(AppDbContext Context, PlayerUser Player, RewardItem Item)> SeedPurchaseDataAsync(
        int gold = 300,
        int cost = 50)
    {
        var context = CreateContext();
        var playerClass = new UserPlayerClass { Name = "Warrior", IsActive = true };
        var person = new Person
        {
            Name = "Test",
            LastName = "User",
            Email = $"test-{Guid.NewGuid()}@example.com",
            IsActive = true,
        };
        var player = new PlayerUser
        {
            Person = person,
            Class = playerClass,
            UserName = $"user-{Guid.NewGuid()}",
            Password = "hash",
            Level = 1,
            ExperiencePoints = 0,
            Gold = gold,
            IsActive = true,
            CreationDate = DateTime.UtcNow,
        };
        var type = new RewardItemType
        {
            Id = RewardItemTypeIds.XpBoost,
            Name = "Boost de XP",
            IsActive = true,
        };
        var item = new RewardItem
        {
            TypeId = RewardItemTypeIds.XpBoost,
            Type = type,
            Name = "Boost XP x2 (1 día)",
            CostGold = cost,
            EffectValue = 2m,
            DurationDays = 1,
            IsActive = true,
        };

        context.UserPlayerClasses.Add(playerClass);
        context.Persons.Add(person);
        context.PlayerUsers.Add(player);
        context.RewardItemTypes.Add(type);
        context.RewardItems.Add(item);
        await context.SaveChangesAsync();

        return (context, player, item);
    }

    private static PlayerInventoryService CreateService(AppDbContext context) =>
        new(
            context,
            new RewardItemRepository(context),
            new PlayerActiveEffectRepository(context),
            new PlayerEffectService(context),
            CreateLevelProgressService());

    [Fact]
    public async Task PurchaseAsync_WhenGoldIsSufficient_DeductsGoldAndAddsInventory()
    {
        var (context, player, item) = await SeedPurchaseDataAsync();
        var service = CreateService(context);

        var result = await service.PurchaseAsync(player.Id, item.Id);

        Assert.Equal(250, result.RemainingGold);
        Assert.Equal(item.Id, result.Inventory.RewardItemId);
        Assert.Equal(1, result.Inventory.Quantity);
        Assert.False(result.Inventory.IsEquipped);

        var updatedPlayer = await context.PlayerUsers.FindAsync(player.Id);
        Assert.Equal(250, updatedPlayer!.Gold);
    }

    [Fact]
    public async Task PurchaseAsync_WhenGoldIsInsufficient_ThrowsInventoryError()
    {
        var (context, player, item) = await SeedPurchaseDataAsync(gold: 10, cost: 50);
        var service = CreateService(context);

        var ex = await Assert.ThrowsAsync<InventoryError>(() => service.PurchaseAsync(player.Id, item.Id));

        Assert.Equal(InventoryFailureKind.InsufficientGold, ex.Kind);
    }

    [Fact]
    public async Task ActivateAsync_XpBoost_CreatesActiveEffectAndSetsEquipped()
    {
        var (context, player, item) = await SeedPurchaseDataAsync();
        var service = CreateService(context);
        var purchase = await service.PurchaseAsync(player.Id, item.Id);

        var result = await service.ActivateAsync(player.Id, purchase.Inventory.Id);

        Assert.True(result.Inventory.IsEquipped);
        Assert.Equal(0, result.Inventory.Quantity);
        Assert.NotNull(result.ActiveEffect);
        Assert.Equal(2m, result.ActiveEffect!.EffectValue);
        Assert.NotNull(result.ActiveEffect.ExpiresAt);
    }

    [Fact]
    public async Task ActivateAsync_ReviveStreak_RestoresPreviousStreak()
    {
        var context = CreateContext();
        var playerClass = new UserPlayerClass { Name = "Warrior", IsActive = true };
        var person = new Person
        {
            Name = "Test",
            LastName = "User",
            Email = $"revive-{Guid.NewGuid()}@example.com",
            IsActive = true,
        };
        var player = new PlayerUser
        {
            Person = person,
            Class = playerClass,
            UserName = $"revive-{Guid.NewGuid()}",
            Password = "hash",
            DaysStreak = 1,
            Gold = 300,
            IsActive = true,
            CreationDate = DateTime.UtcNow,
        };
        var recoveryType = new RewardItemType
        {
            Id = RewardItemTypeIds.Recovery,
            Name = "Recuperación",
            IsActive = true,
        };
        var reviveItem = new RewardItem
        {
            TypeId = RewardItemTypeIds.Recovery,
            Type = recoveryType,
            Name = "Revivir Racha",
            CostGold = 200,
            IsActive = true,
        };

        context.UserPlayerClasses.Add(playerClass);
        context.Persons.Add(person);
        context.PlayerUsers.Add(player);
        context.RewardItemTypes.Add(recoveryType);
        context.RewardItems.Add(reviveItem);
        context.PlayerEvents.Add(new PlayerEvent
        {
            PlayerUser = player,
            EventType = PlayerEventType.STREAK_RESET,
            PayloadJson = "{\"previousStreak\":12,\"newStreak\":1}",
            CreatedAt = DateTime.UtcNow,
        });
        context.PlayerInventories.Add(new PlayerInventory
        {
            PlayerUser = player,
            RewardItem = reviveItem,
            Quantity = 1,
            AcquiredAt = DateTime.UtcNow,
        });
        await context.SaveChangesAsync();

        var inventory = await context.PlayerInventories.FirstAsync();
        var service = CreateService(context);

        var result = await service.ActivateAsync(player.Id, inventory.Id);

        var updatedPlayer = await context.PlayerUsers.FindAsync(player.Id);
        Assert.Equal(12, updatedPlayer!.DaysStreak);
        Assert.Contains("12", result.RecoveryMessage);
    }
}
