using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.Infrastructure.Errors;
using LevelUpLifeBackend.Models;
using LevelUpLifeBackend.Repositories;
using LevelUpLifeBackend.Services;
using Moq;
using Xunit;

namespace LevelUpLife_Backend.Tests;

public class HabitDisciplineServiceTests
{
    private readonly Mock<IHabitDisciplineRepository> _repositoryMock;
    private readonly Mock<IHabitCategoryRepository> _categoryRepositoryMock;
    private readonly HabitDisciplineService _service;

    public HabitDisciplineServiceTests()
    {
        _repositoryMock = new Mock<IHabitDisciplineRepository>();
        _categoryRepositoryMock = new Mock<IHabitCategoryRepository>();
        _service = new HabitDisciplineService(
            _repositoryMock.Object,
            _categoryRepositoryMock.Object);
    }

    [Fact]
    public async Task GetDisciplineByIdAsync_WhenDisciplineExists_ReturnsDto()
    {
        var discipline = new HabitDiscipline
        {
            Id = 7,
            Name = "Strength",
            Description = "Physical training",
            IsActive = true,
            Category = new HabitCategory { Id = 3 },
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(7)).ReturnsAsync(discipline);

        var result = await _service.GetDisciplineByIdAsync(7);

        Assert.NotNull(result);
        Assert.Equal(7, result.IdHabitDiscipline);
        Assert.Equal(3, result.IdHabitCategory);
        Assert.Equal("Strength", result.DscHabitDisciplineName);
        Assert.Equal("Physical training", result.DscHabitDisciplineDescription);
        Assert.True(result.StatusHabitDisciplineIsActive);
    }

    [Fact]
    public async Task GetDisciplineByIdAsync_WhenDisciplineDoesNotExist_ThrowsNotFoundError()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((HabitDiscipline?)null);

        var exception = await Assert.ThrowsAsync<NotFoundError>(() => _service.GetDisciplineByIdAsync(999));

        Assert.Equal(404, exception.HttpStatusCode);
        Assert.Equal(404, exception.Payload.Code);
    }

    [Fact]
    public async Task GetDisciplineByIdAsync_WhenRepositoryThrowsException_ThrowsServerError()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ThrowsAsync(new Exception("Database error"));

        var exception = await Assert.ThrowsAsync<ServerError>(() => _service.GetDisciplineByIdAsync(1));

        Assert.Equal(500, exception.HttpStatusCode);
    }

    [Fact]
    public async Task CreateDisciplineAsync_WhenCategoryExists_ReturnsCreatedDto()
    {
        var request = new CreateHabitDisciplineRequestDto
        {
            IdHabitCategory = 2,
            DscHabitDisciplineName = "Meditation",
            DscHabitDisciplineDescription = "Daily mindfulness practice",
            StatusHabitDisciplineIsActive = true,
        };

        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new HabitCategory { Id = 2 });
        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<HabitDiscipline>()))
            .ReturnsAsync((HabitDiscipline discipline) =>
            {
                discipline.Id = 15;
                return discipline;
            });

        var result = await _service.CreateDisciplineAsync(request);

        Assert.Equal(15, result.IdHabitDiscipline);
        Assert.Equal(2, result.IdHabitCategory);
        Assert.Equal("Meditation", result.DscHabitDisciplineName);
        Assert.Equal("Daily mindfulness practice", result.DscHabitDisciplineDescription);
        Assert.True(result.StatusHabitDisciplineIsActive);
    }

    [Fact]
    public async Task CreateDisciplineAsync_WhenCategoryDoesNotExist_ThrowsAppError400()
    {
        var request = new CreateHabitDisciplineRequestDto
        {
            IdHabitCategory = 999,
            DscHabitDisciplineName = "Meditation",
        };

        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((HabitCategory?)null);

        var exception = await Assert.ThrowsAsync<AppError>(() => _service.CreateDisciplineAsync(request));

        Assert.Equal(400, exception.HttpStatusCode);
    }

    [Fact]
    public async Task UpdateDisciplineAsync_WhenDisciplineExists_ReturnsUpdatedDto()
    {
        var existing = new HabitDiscipline
        {
            Id = 7,
            Name = "Strength",
            Description = "Old description",
            IsActive = true,
            Category = new HabitCategory { Id = 3 },
        };

        var request = new UpdateHabitDisciplineRequestDto
        {
            IdHabitCategory = 2,
            DscHabitDisciplineName = "Cardio",
            DscHabitDisciplineDescription = "Updated description",
            StatusHabitDisciplineIsActive = false,
        };

        _repositoryMock.Setup(r => r.GetByIdForUpdateAsync(7)).ReturnsAsync(existing);
        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new HabitCategory { Id = 2 });
        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<HabitDiscipline>(), It.IsAny<int>()))
            .ReturnsAsync((HabitDiscipline discipline, int categoryId) =>
            {
                discipline.Category = new HabitCategory { Id = categoryId };
                return discipline;
            });

        var result = await _service.UpdateDisciplineAsync(7, request);

        Assert.Equal(7, result.IdHabitDiscipline);
        Assert.Equal(2, result.IdHabitCategory);
        Assert.Equal("Cardio", result.DscHabitDisciplineName);
        Assert.Equal("Updated description", result.DscHabitDisciplineDescription);
        Assert.False(result.StatusHabitDisciplineIsActive);
    }

    [Fact]
    public async Task UpdateDisciplineAsync_WhenDisciplineDoesNotExist_ThrowsNotFoundError()
    {
        var request = new UpdateHabitDisciplineRequestDto
        {
            IdHabitCategory = 1,
            DscHabitDisciplineName = "Cardio",
        };

        _repositoryMock.Setup(r => r.GetByIdForUpdateAsync(999)).ReturnsAsync((HabitDiscipline?)null);

        var exception = await Assert.ThrowsAsync<NotFoundError>(
            () => _service.UpdateDisciplineAsync(999, request));

        Assert.Equal(404, exception.HttpStatusCode);
    }

    [Fact]
    public async Task UpdateDisciplineAsync_WhenCategoryDoesNotExist_ThrowsAppError400()
    {
        var existing = new HabitDiscipline
        {
            Id = 7,
            Name = "Strength",
            Category = new HabitCategory { Id = 3 },
        };

        var request = new UpdateHabitDisciplineRequestDto
        {
            IdHabitCategory = 999,
            DscHabitDisciplineName = "Cardio",
        };

        _repositoryMock.Setup(r => r.GetByIdForUpdateAsync(7)).ReturnsAsync(existing);
        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((HabitCategory?)null);

        var exception = await Assert.ThrowsAsync<AppError>(
            () => _service.UpdateDisciplineAsync(7, request));

        Assert.Equal(400, exception.HttpStatusCode);
    }

    [Fact]
    public async Task UpdateDisciplineAsync_WhenRepositoryThrowsException_ThrowsServerError()
    {
        var request = new UpdateHabitDisciplineRequestDto
        {
            IdHabitCategory = 1,
            DscHabitDisciplineName = "Cardio",
        };

        _repositoryMock.Setup(r => r.GetByIdForUpdateAsync(1)).ThrowsAsync(new Exception("Database error"));

        var exception = await Assert.ThrowsAsync<ServerError>(
            () => _service.UpdateDisciplineAsync(1, request));

        Assert.Equal(500, exception.HttpStatusCode);
    }
}
