using LevelUpLifeBackend.Data;
using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.Infrastructure.Errors;
using LevelUpLifeBackend.Models;
using LevelUpLifeBackend.Repositories;
using LevelUpLifeBackend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Xunit;

namespace LevelUpLife_Backend.Tests;

public class HabitServiceUpdateTaskTests
{
    private static AppDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new AppDbContext(options);
    }

    private static HabitService CreateService(
        AppDbContext context,
        Mock<IHabitTaskRepository> taskRepo,
        Mock<IStreakLogRepository>? streakRepo = null,
        Mock<IPlayerEventRepository>? playerEventRepo = null
    )
    {
        var habitRepo = new Mock<IHabitRepository>();
        var criteriaRepo = new Mock<IRepetitionCriteriaRepository>();
        var timerRepo = new Mock<ITimerCriteriaRepository>();
        var streakLogRepo = streakRepo ?? CreateDefaultStreakRepoMock();
        var playerEvents = playerEventRepo ?? new Mock<IPlayerEventRepository>();
        playerEvents
            .Setup(r => r.AddAsync(It.IsAny<PlayerEvent>()))
            .Returns(Task.CompletedTask);

        return new HabitService(
            habitRepo.Object,
            taskRepo.Object,
            criteriaRepo.Object,
            timerRepo.Object,
            streakLogRepo.Object,
            playerEvents.Object,
            context
        );
    }

    private static Mock<IStreakLogRepository> CreateDefaultStreakRepoMock()
    {
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
        return streakRepo;
    }

    private static HabitTask SampleTask(int taskId = 1, int habitId = 10, int userId = 2)
    {
        var user = new PlayerUser { Id = userId, UserName = "testuser" };
        var category = new HabitCategory { Id = 1, Name = "Salud" };
        var discipline = new HabitDiscipline
        {
            Id = 3,
            Name = "Ejercicio",
            Category = category,
        };
        var habit = new Habit
        {
            Id = habitId,
            Title = "Habit",
            IsActive = true,
            User = user,
            Discipline = discipline,
        };

        return new HabitTask
        {
            Id = taskId,
            HabitId = habitId,
            Habit = habit,
            HabitDisciplineId = discipline.Id,
            Title = "Old title",
            Difficulty = TaskDifficulty.EASY,
            Frequency = TaskFrequency.DAILY,
            PeriodLength = 1,
            PeriodUnit = TaskPeriodUnit.DAYS,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            CompletionCriteria = TaskCompletionCriteria.REPETITIONS,
            IsActive = true,
            RepetitionCriteria = new RepetitionCriteria
            {
                Id = 5,
                HabitTaskId = taskId,
                NumRepetitionsObjective = 2,
                TypeUnityMeasurementUnit = MeasurementUnit.SERIES,
            },
        };
    }

    private static CreateStandaloneHabitTaskRequestDto ValidUpdateRequest(int habitId = 10) =>
        new()
        {
            HabitId = habitId,
            Title = "Updated title",
            Difficulty = TaskDifficulty.HARD,
            Frequency = TaskFrequency.WEEKLY,
            PeriodLength = 2,
            PeriodUnit = TaskPeriodUnit.WEEKS,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            CompletionCriteria = TaskCompletionCriteria.REPETITIONS,
            RepetitionCriteria = new CreateRepetitionCriteriaRequestDto
            {
                Repetitions = 5,
                MeasurementUnit = MeasurementUnit.REPS,
                IsPartialAllowed = true,
            },
        };

    [Fact]
    public async Task UpdateTaskAsync_WhenTaskExists_ReturnsUpdatedTask()
    {
        await using var context = CreateInMemoryContext();
        var task = SampleTask();
        var taskRepo = new Mock<IHabitTaskRepository>();

        taskRepo.Setup(r => r.GetTrackedByIdForUserAsync(1, 2)).ReturnsAsync(task);
        taskRepo
            .Setup(r => r.UpdateWithRepetitionCriteriaAsync(It.IsAny<HabitTask>()))
            .Returns(Task.CompletedTask);

        var service = CreateService(context, taskRepo);
        var result = await service.UpdateTaskAsync(1, ValidUpdateRequest(), userId: 2);

        Assert.Equal("Updated title", result.Title);
        Assert.Equal(TaskDifficulty.HARD, result.Difficulty);
        Assert.NotNull(result.RepetitionCriteria);
        Assert.Equal(5, result.RepetitionCriteria!.Repetitions);
        taskRepo.Verify(r => r.UpdateWithRepetitionCriteriaAsync(It.IsAny<HabitTask>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTaskAsync_WhenTaskNotFound_ThrowsNotFoundError()
    {
        await using var context = CreateInMemoryContext();
        var taskRepo = new Mock<IHabitTaskRepository>();
        taskRepo.Setup(r => r.GetTrackedByIdForUserAsync(99, 2)).ReturnsAsync((HabitTask?)null);

        var service = CreateService(context, taskRepo);

        await Assert.ThrowsAsync<NotFoundError>(() =>
            service.UpdateTaskAsync(99, ValidUpdateRequest(), userId: 2)
        );
    }

    [Fact]
    public async Task UpdateTaskAsync_WhenHabitIdMismatch_ThrowsAppError400()
    {
        await using var context = CreateInMemoryContext();
        var task = SampleTask(habitId: 10);
        var taskRepo = new Mock<IHabitTaskRepository>();
        taskRepo.Setup(r => r.GetTrackedByIdForUserAsync(1, 2)).ReturnsAsync(task);

        var service = CreateService(context, taskRepo);

        var ex = await Assert.ThrowsAsync<AppError>(() =>
            service.UpdateTaskAsync(1, ValidUpdateRequest(habitId: 999), userId: 2)
        );
        Assert.Equal(400, ex.HttpStatusCode);
    }

    [Fact]
    public async Task DeactivateTaskAsync_WhenTaskExists_SetsIsActiveToFalse()
    {
        await using var context = CreateInMemoryContext();
        var task = SampleTask();
        var taskRepo = new Mock<IHabitTaskRepository>();

        taskRepo.Setup(r => r.GetTrackedByIdForUserAsync(1, 2)).ReturnsAsync(task);
        taskRepo.Setup(r => r.UpdateAsync(task)).Returns(Task.CompletedTask);

        var service = CreateService(context, taskRepo);
        await service.DeactivateTaskAsync(1, userId: 2);

        Assert.False(task.IsActive);
        taskRepo.Verify(r => r.UpdateAsync(task), Times.Once);
    }

    [Fact]
    public async Task DeactivateTaskAsync_WhenTaskAlreadyInactive_SkipsRepositoryUpdate()
    {
        await using var context = CreateInMemoryContext();
        var task = SampleTask();
        task.IsActive = false;
        var taskRepo = new Mock<IHabitTaskRepository>();

        taskRepo.Setup(r => r.GetTrackedByIdForUserAsync(1, 2)).ReturnsAsync(task);

        var service = CreateService(context, taskRepo);
        await service.DeactivateTaskAsync(1, userId: 2);

        Assert.False(task.IsActive);
        taskRepo.Verify(r => r.UpdateAsync(It.IsAny<HabitTask>()), Times.Never);
    }

    [Fact]
    public async Task DeactivateTaskAsync_WhenTaskNotFound_ThrowsNotFoundError()
    {
        await using var context = CreateInMemoryContext();
        var taskRepo = new Mock<IHabitTaskRepository>();
        taskRepo.Setup(r => r.GetTrackedByIdForUserAsync(99, 2)).ReturnsAsync((HabitTask?)null);

        var service = CreateService(context, taskRepo);

        await Assert.ThrowsAsync<NotFoundError>(() =>
            service.DeactivateTaskAsync(99, userId: 2)
        );
    }

    [Fact]
    public async Task DeactivateTaskAsync_WhenHabitIsInactive_ThrowsNotFoundError()
    {
        await using var context = CreateInMemoryContext();
        var task = SampleTask();
        task.Habit!.IsActive = false;
        var taskRepo = new Mock<IHabitTaskRepository>();
        taskRepo.Setup(r => r.GetTrackedByIdForUserAsync(1, 2)).ReturnsAsync(task);

        var service = CreateService(context, taskRepo);

        await Assert.ThrowsAsync<NotFoundError>(() =>
            service.DeactivateTaskAsync(1, userId: 2)
        );
    }

    [Fact]
    public async Task CompleteTaskAsync_WhenTaskExists_ReturnsXpLevelAndStreak()
    {
        await using var context = CreateInMemoryContext();
        var task = SampleTask();
        task.XpValue = 50;
        var taskRepo = new Mock<IHabitTaskRepository>();
        var streakRepo = CreateDefaultStreakRepoMock();
        streakRepo
            .Setup(r => r.GetByPlayerAndDateAsync(2, It.IsAny<DateOnly>()))
            .ReturnsAsync((StreakLog?)null);
        streakRepo
            .Setup(r => r.GetLastCompletionBeforeAsync(2, It.IsAny<DateOnly>()))
            .ReturnsAsync((StreakLog?)null);

        taskRepo.Setup(r => r.GetTrackedByIdForUserAsync(1, 2)).ReturnsAsync(task);
        taskRepo
            .Setup(r => r.CompleteTaskAsync(It.IsAny<HabitTask>(), It.IsAny<StreakLog?>()))
            .Returns(Task.CompletedTask);
        streakRepo
            .Setup(r => r.GetByPlayerAndDateAsync(2, It.IsAny<DateOnly>()))
            .ReturnsAsync((StreakLog?)null);
        streakRepo
            .Setup(r => r.GetLastCompletionBeforeAsync(2, It.IsAny<DateOnly>()))
            .ReturnsAsync((StreakLog?)null);

        var service = CreateService(context, taskRepo, streakRepo);
        var result = await service.CompleteTaskAsync(
            1,
            userId: 2,
            new CompleteHabitTaskRequestDto { CompletedAt = DateTime.UtcNow }
        );

        Assert.True(task.IsCompleted);
        Assert.Equal(50, task.EarnedXpSnapshot);
        Assert.Equal(50, result.XpEarned);
        Assert.Equal(1, result.NewLevel);
        Assert.True(result.StreakUpdated);
        Assert.Equal(50, task.Habit!.User.ExperiencePoints);
        taskRepo.Verify(r => r.CompleteTaskAsync(task, It.IsAny<StreakLog>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTaskAsync_WhenTaskCompleted_PreservesCompletionState()
    {
        await using var context = CreateInMemoryContext();
        var task = SampleTask();
        task.IsCompleted = true;
        task.EarnedXpSnapshot = 25;
        task.XpValue = 999;

        var taskRepo = new Mock<IHabitTaskRepository>();
        taskRepo.Setup(r => r.GetTrackedByIdForUserAsync(1, 2)).ReturnsAsync(task);
        taskRepo
            .Setup(r => r.UpdateWithRepetitionCriteriaAsync(It.IsAny<HabitTask>()))
            .Returns(Task.CompletedTask);

        var service = CreateService(context, taskRepo);
        var request = ValidUpdateRequest();
        request.XpValue = 999;

        var result = await service.UpdateTaskAsync(1, request, userId: 2);

        Assert.True(task.IsCompleted);
        Assert.Equal(25, task.EarnedXpSnapshot);
        Assert.True(result.IsCompleted);
    }

    [Fact]
    public async Task CompleteTaskAsync_WhenAlreadyCompleted_ThrowsTaskError()
    {
        await using var context = CreateInMemoryContext();
        var task = SampleTask();
        task.IsCompleted = true;
        var taskRepo = new Mock<IHabitTaskRepository>();
        taskRepo.Setup(r => r.GetTrackedByIdForUserAsync(1, 2)).ReturnsAsync(task);

        var service = CreateService(context, taskRepo);

        var ex = await Assert.ThrowsAsync<TaskError>(() =>
            service.CompleteTaskAsync(
                1,
                userId: 2,
                new CompleteHabitTaskRequestDto { CompletedAt = DateTime.UtcNow }
            )
        );

        Assert.Equal(TaskFailureKind.CompletionRequirementsNotMet, ex.Kind);
        Assert.Equal(400, ex.HttpStatusCode);
    }

    [Fact]
    public async Task CompleteTaskAsync_WhenTaskNotFound_ThrowsNotFoundError()
    {
        await using var context = CreateInMemoryContext();
        var taskRepo = new Mock<IHabitTaskRepository>();
        taskRepo.Setup(r => r.GetTrackedByIdForUserAsync(99, 2)).ReturnsAsync((HabitTask?)null);

        var service = CreateService(context, taskRepo);

        await Assert.ThrowsAsync<NotFoundError>(() =>
            service.CompleteTaskAsync(
                99,
                userId: 2,
                new CompleteHabitTaskRequestDto { CompletedAt = DateTime.UtcNow }
            )
        );
    }

    [Fact]
    public async Task CompleteTaskAsync_WhenEvidenceMissing_ThrowsTaskError()
    {
        await using var context = CreateInMemoryContext();
        var task = SampleTask();
        task.CompletionCriteria = TaskCompletionCriteria.EVIDENCE;
        task.Evidence = TaskEvidence.PHOTO;
        var taskRepo = new Mock<IHabitTaskRepository>();

        taskRepo.Setup(r => r.GetTrackedByIdForUserAsync(1, 2)).ReturnsAsync(task);
        taskRepo.Setup(r => r.GetEvidencesByTaskIdAsync(1)).ReturnsAsync(Array.Empty<EvidenceStorage>());

        var service = CreateService(context, taskRepo);

        await Assert.ThrowsAsync<TaskError>(() =>
            service.CompleteTaskAsync(
                1,
                userId: 2,
                new CompleteHabitTaskRequestDto { CompletedAt = DateTime.UtcNow }
            )
        );
    }

    [Fact]
    public async Task CompleteTaskAsync_WhenLevelUp_ReturnsNewLevel()
    {
        await using var context = CreateInMemoryContext();
        var task = SampleTask();
        task.XpValue = 100;
        task.Habit!.User.ExperiencePoints = 50;
        task.Habit.User.Level = 1;
        var taskRepo = new Mock<IHabitTaskRepository>();
        var streakRepo = CreateDefaultStreakRepoMock();
        streakRepo
            .Setup(r => r.GetByPlayerAndDateAsync(2, It.IsAny<DateOnly>()))
            .ReturnsAsync((StreakLog?)null);
        streakRepo
            .Setup(r => r.GetLastCompletionBeforeAsync(2, It.IsAny<DateOnly>()))
            .ReturnsAsync((StreakLog?)null);

        taskRepo.Setup(r => r.GetTrackedByIdForUserAsync(1, 2)).ReturnsAsync(task);
        taskRepo
            .Setup(r => r.CompleteTaskAsync(It.IsAny<HabitTask>(), It.IsAny<StreakLog?>()))
            .Returns(Task.CompletedTask);
        streakRepo
            .Setup(r => r.GetByPlayerAndDateAsync(2, It.IsAny<DateOnly>()))
            .ReturnsAsync((StreakLog?)null);
        streakRepo
            .Setup(r => r.GetLastCompletionBeforeAsync(2, It.IsAny<DateOnly>()))
            .ReturnsAsync((StreakLog?)null);

        var service = CreateService(context, taskRepo, streakRepo);
        var result = await service.CompleteTaskAsync(
            1,
            userId: 2,
            new CompleteHabitTaskRequestDto { CompletedAt = DateTime.UtcNow }
        );

        Assert.Equal(2, result.NewLevel);
        Assert.Equal(100, result.XpEarned);
    }

    [Fact]
    public async Task CompleteTaskAsync_WhenRepetitionCriteriaMissing_ThrowsTaskError()
    {
        await using var context = CreateInMemoryContext();
        var task = SampleTask();
        task.CompletionCriteria = TaskCompletionCriteria.REPETITIONS;
        task.RepetitionCriteria = null;
        var taskRepo = new Mock<IHabitTaskRepository>();
        taskRepo.Setup(r => r.GetTrackedByIdForUserAsync(1, 2)).ReturnsAsync(task);

        var service = CreateService(context, taskRepo);

        await Assert.ThrowsAsync<TaskError>(() =>
            service.CompleteTaskAsync(
                1,
                userId: 2,
                new CompleteHabitTaskRequestDto { CompletedAt = DateTime.UtcNow }
            )
        );
    }

    [Fact]
    public async Task CompleteTaskAsync_WhenTimerCriteriaMissing_ThrowsTaskError()
    {
        await using var context = CreateInMemoryContext();
        var task = SampleTask();
        task.CompletionCriteria = TaskCompletionCriteria.TIMER;
        task.TimerCriteria = null;
        var taskRepo = new Mock<IHabitTaskRepository>();
        taskRepo.Setup(r => r.GetTrackedByIdForUserAsync(1, 2)).ReturnsAsync(task);

        var service = CreateService(context, taskRepo);

        await Assert.ThrowsAsync<TaskError>(() =>
            service.CompleteTaskAsync(
                1,
                userId: 2,
                new CompleteHabitTaskRequestDto { CompletedAt = DateTime.UtcNow }
            )
        );
    }
}
