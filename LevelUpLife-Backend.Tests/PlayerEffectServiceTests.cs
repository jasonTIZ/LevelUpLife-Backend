using LevelUpLifeBackend.Data;
using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.Infrastructure;
using LevelUpLifeBackend.Infrastructure.Configuration;
using LevelUpLifeBackend.Models;
using LevelUpLifeBackend.Repositories;
using LevelUpLifeBackend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace LevelUpLife_Backend.Tests;

public class PlayerEffectServiceTests
{
    [Theory]
    [InlineData(100, 25)]
    [InlineData(500, 50)]
    public void CalculateXpPenalty_UsesTenPercentWithMinimum(int currentXp, int expectedPenalty)
    {
        var service = new PlayerEffectService(CreateContext());
        Assert.Equal(expectedPenalty, service.CalculateXpPenalty(currentXp));
    }

    [Fact]
    public async Task GetActiveXpMultiplierAsync_ReturnsHighestActiveBoost()
    {
        var context = CreateContext();
        var player = await SeedPlayerAsync(context);
        context.PlayerActiveEffects.AddRange(
            new PlayerActiveEffect
            {
                PlayerUserId = player.Id,
                InventoryId = 1,
                RewardItemId = 1,
                RewardItemTypeId = RewardItemTypeIds.XpBoost,
                EffectValue = 1.5m,
                ActivatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                IsActive = true,
            },
            new PlayerActiveEffect
            {
                PlayerUserId = player.Id,
                InventoryId = 2,
                RewardItemId = 2,
                RewardItemTypeId = RewardItemTypeIds.XpBoost,
                EffectValue = 2m,
                ActivatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                IsActive = true,
            });
        await context.SaveChangesAsync();

        var service = new PlayerEffectService(context);
        var multiplier = await service.GetActiveXpMultiplierAsync(player.Id);

        Assert.Equal(2m, multiplier);
    }

    [Fact]
    public async Task TryConsumeStreakShieldAsync_WhenChargesAreEnough_ReturnsTrueAndDecrements()
    {
        var context = CreateContext();
        var player = await SeedPlayerAsync(context);
        var inventory = new PlayerInventory
        {
            PlayerUserId = player.Id,
            RewardItemId = 1,
            Quantity = 0,
            IsEquipped = true,
            AcquiredAt = DateTime.UtcNow,
        };
        context.PlayerInventories.Add(inventory);
        await context.SaveChangesAsync();

        context.PlayerActiveEffects.Add(new PlayerActiveEffect
        {
            PlayerUserId = player.Id,
            InventoryId = inventory.Id,
            RewardItemId = 1,
            RewardItemTypeId = RewardItemTypeIds.StreakProtection,
            EffectValue = 2m,
            RemainingCharges = 2,
            ActivatedAt = DateTime.UtcNow,
            IsActive = true,
            Inventory = inventory,
        });
        await context.SaveChangesAsync();

        var service = new PlayerEffectService(context);
        var consumed = await service.TryConsumeStreakShieldAsync(player.Id, 1);

        Assert.True(consumed);
        var effect = await context.PlayerActiveEffects.SingleAsync();
        Assert.Equal(1, effect.RemainingCharges);
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new AppDbContext(options);
    }

    private static async Task<PlayerUser> SeedPlayerAsync(AppDbContext context)
    {
        var playerClass = new UserPlayerClass { Name = "Warrior", IsActive = true };
        var person = new Person
        {
            Name = "Test",
            LastName = "User",
            Email = $"effect-{Guid.NewGuid()}@example.com",
            IsActive = true,
        };
        var player = new PlayerUser
        {
            Person = person,
            Class = playerClass,
            UserName = $"effect-{Guid.NewGuid()}",
            Password = "hash",
            IsActive = true,
            CreationDate = DateTime.UtcNow,
        };

        context.UserPlayerClasses.Add(playerClass);
        context.Persons.Add(person);
        context.PlayerUsers.Add(player);
        await context.SaveChangesAsync();
        return player;
    }
}

public class HabitServiceRewardEffectsTests
{
    [Fact]
    public async Task CompleteTaskAsync_AppliesActiveXpMultiplier()
    {
        var context = CreateContext();
        var (player, task) = await SeedCompletionDataAsync(context);
        context.PlayerActiveEffects.Add(new PlayerActiveEffect
        {
            PlayerUserId = player.Id,
            InventoryId = 1,
            RewardItemId = 1,
            RewardItemTypeId = RewardItemTypeIds.XpBoost,
            EffectValue = 2m,
            ActivatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            IsActive = true,
        });
        await context.SaveChangesAsync();

        var effectService = new PlayerEffectService(context);
        var streakRepo = new Mock<IStreakLogRepository>();
        streakRepo
            .Setup(r => r.GetByPlayerAndDateAsync(It.IsAny<int>(), It.IsAny<DateOnly>()))
            .ReturnsAsync((StreakLog?)null);
        streakRepo
            .Setup(r => r.GetLastCompletionBeforeAsync(It.IsAny<int>(), It.IsAny<DateOnly>()))
            .ReturnsAsync((StreakLog?)null);
        streakRepo
            .Setup(r => r.HasProtectionInGapAsync(It.IsAny<int>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>()))
            .ReturnsAsync(false);

        var taskRepo = new Mock<IHabitTaskRepository>();
        taskRepo
            .Setup(r => r.GetTrackedByIdForUserAsync(task.Id, player.Id))
            .ReturnsAsync(task);
        taskRepo
            .Setup(r => r.CompleteTaskAsync(task, It.IsAny<StreakLog?>(), It.IsAny<IReadOnlyList<PlayerEvent>?>()))
            .Returns(Task.CompletedTask);

        var service = new HabitService(
            new Mock<IHabitRepository>().Object,
            taskRepo.Object,
            new Mock<IRepetitionCriteriaRepository>().Object,
            new Mock<ITimerCriteriaRepository>().Object,
            streakRepo.Object,
            CreateLevelProgressService(),
            effectService,
            context);

        var response = await service.CompleteTaskAsync(
            task.Id,
            player.Id,
            new CompleteHabitTaskRequestDto { CompletedAt = DateTime.UtcNow });

        Assert.Equal(100, response.XpEarned);
        Assert.Equal(100, task.EarnedXpSnapshot);
        Assert.Equal(100, player.Gold);
    }

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

    private static async Task<(PlayerUser Player, HabitTask Task)> SeedCompletionDataAsync(AppDbContext context)
    {
        var playerClass = new UserPlayerClass { Name = "Warrior", IsActive = true };
        var person = new Person
        {
            Name = "Test",
            LastName = "User",
            Email = $"complete-{Guid.NewGuid()}@example.com",
            IsActive = true,
        };
        var player = new PlayerUser
        {
            Person = person,
            Class = playerClass,
            UserName = $"complete-{Guid.NewGuid()}",
            Password = "hash",
            ExperiencePoints = 0,
            Gold = 0,
            DaysStreak = 0,
            IsActive = true,
            CreationDate = DateTime.UtcNow,
        };
        var category = new HabitCategory { Name = "Health", IsActive = true };
        var discipline = new HabitDiscipline { Category = category, Name = "Fitness", IsActive = true };
        var habit = new Habit
        {
            Discipline = discipline,
            User = player,
            Title = "Run",
            IsActive = true,
        };
        var task = new HabitTask
        {
            Habit = habit,
            Title = "Morning run",
            Difficulty = TaskDifficulty.HARD,
            XpValue = 0,
            Frequency = TaskFrequency.DAILY,
            PeriodUnit = TaskPeriodUnit.DAYS,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
            CompletionCriteria = TaskCompletionCriteria.REPETITIONS,
            Evidence = TaskEvidence.PHOTO,
            IsActive = true,
            RepetitionCriteria = new RepetitionCriteria
            {
                NumRepetitionsObjective = 10,
                TypeUnityMeasurementUnit = MeasurementUnit.REPS,
                StatusRepetitionsCriteriaIsActive = true,
            },
        };

        context.UserPlayerClasses.Add(playerClass);
        context.Persons.Add(person);
        context.PlayerUsers.Add(player);
        context.HabitCategories.Add(category);
        context.Disciplines.Add(discipline);
        context.Habits.Add(habit);
        context.HabitTasks.Add(task);
        await context.SaveChangesAsync();

        return (player, task);
    }
}
