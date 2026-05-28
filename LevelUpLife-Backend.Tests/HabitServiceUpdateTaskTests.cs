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
        Mock<IRepetitionCriteriaRepository>? criteriaRepo = null
    )
    {
        var habitRepo = new Mock<IHabitRepository>();
        var criteria = criteriaRepo ?? new Mock<IRepetitionCriteriaRepository>();
        return new HabitService(habitRepo.Object, taskRepo.Object, criteria.Object, context);
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
        var criteriaRepo = new Mock<IRepetitionCriteriaRepository>();

        taskRepo
            .Setup(r => r.GetTrackedByIdForUserAsync(1, 2))
            .ReturnsAsync(task);
        taskRepo.Setup(r => r.UpdateAsync(It.IsAny<HabitTask>())).Returns(Task.CompletedTask);
        criteriaRepo
            .Setup(r => r.UpdateAsync(It.IsAny<RepetitionCriteria>()))
            .ReturnsAsync((RepetitionCriteria c) => c);

        var service = CreateService(context, taskRepo, criteriaRepo);
        var request = ValidUpdateRequest();

        var result = await service.UpdateTaskAsync(1, request, userId: 2);

        Assert.Equal("Updated title", result.Title);
        Assert.Equal(TaskDifficulty.HARD, result.Difficulty);
        Assert.NotNull(result.RepetitionCriteria);
        Assert.Equal(5, result.RepetitionCriteria!.Repetitions);
        taskRepo.Verify(r => r.UpdateAsync(It.IsAny<HabitTask>()), Times.Once);
        criteriaRepo.Verify(r => r.UpdateAsync(It.IsAny<RepetitionCriteria>()), Times.Once);
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
        var request = ValidUpdateRequest(habitId: 999);

        var ex = await Assert.ThrowsAsync<AppError>(() =>
            service.UpdateTaskAsync(1, request, userId: 2)
        );
        Assert.Equal(400, ex.HttpStatusCode);
    }
}
