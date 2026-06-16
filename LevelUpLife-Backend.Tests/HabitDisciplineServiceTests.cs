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
    public async Task GetAllDisciplinesAsync_WhenDisciplinesExist_ReturnsMappedDtos()
    {
        var disciplines = new List<HabitDiscipline>
        {
            new()
            {
                Id = 1,
                Name = "Strength",
                Description = "Physical training",
                IsActive = true,
                Category = new HabitCategory { Id = 3 },
            },
            new()
            {
                Id = 2,
                Name = "Meditation",
                Description = "Mindfulness practice",
                IsActive = false,
                Category = new HabitCategory { Id = 5 },
            },
        };

        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(disciplines);

        var result = (await _service.GetAllDisciplinesAsync()).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].IdHabitDiscipline);
        Assert.Equal(3, result[0].IdHabitCategory);
        Assert.Equal("Strength", result[0].DscHabitDisciplineName);
        Assert.Equal("Physical training", result[0].DscHabitDisciplineDescription);
        Assert.True(result[0].StatusHabitDisciplineIsActive);
        Assert.Equal(2, result[1].IdHabitDiscipline);
        Assert.Equal(5, result[1].IdHabitCategory);
        Assert.False(result[1].StatusHabitDisciplineIsActive);
    }

    [Fact]
    public async Task GetAllDisciplinesAsync_WhenRepositoryThrowsException_ThrowsServerError()
    {
        _repositoryMock.Setup(r => r.GetAllAsync()).ThrowsAsync(new Exception("Database error"));

        var exception = await Assert.ThrowsAsync<ServerError>(() => _service.GetAllDisciplinesAsync());

        Assert.Equal(500, exception.HttpStatusCode);
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
    public async Task UpdateDisciplineStatusAsync_WhenDisciplineExists_DeactivatesAndReturnsSuccess()
    {
        var discipline = new HabitDiscipline
        {
            Id = 3,
            Name = "Strength",
            Description = "Physical training",
            IsActive = true,
            Category = new HabitCategory { Id = 1 },
        };

        _repositoryMock.Setup(r => r.GetTrackedByIdAsync(3)).ReturnsAsync(discipline);
        _repositoryMock.Setup(r => r.UpdateAsync(discipline)).Returns(Task.CompletedTask);

        var request = new UpdateHabitDisciplineStatusRequestDto
        {
            StatusHabitDisciplineIsActive = false,
        };

        var (success, message) = await _service.UpdateDisciplineStatusAsync(3, request);

        Assert.True(success);
        Assert.Equal("Discipline deactivated successfully.", message);
        Assert.False(discipline.IsActive);
        _repositoryMock.Verify(r => r.UpdateAsync(discipline), Times.Once);
    }

    [Fact]
    public async Task UpdateDisciplineStatusAsync_WhenDisciplineDoesNotExist_ThrowsNotFoundError()
    {
        _repositoryMock.Setup(r => r.GetTrackedByIdAsync(999)).ReturnsAsync((HabitDiscipline?)null);

        var request = new UpdateHabitDisciplineStatusRequestDto
        {
            StatusHabitDisciplineIsActive = false,
        };

        var exception = await Assert.ThrowsAsync<NotFoundError>(
            () => _service.UpdateDisciplineStatusAsync(999, request));

        Assert.Equal(404, exception.HttpStatusCode);
        Assert.Equal(404, exception.Payload.Code);
    }

    [Fact]
    public async Task UpdateDisciplineStatusAsync_WhenRepositoryThrowsException_ThrowsServerError()
    {
        _repositoryMock
            .Setup(r => r.GetTrackedByIdAsync(1))
            .ThrowsAsync(new Exception("Database error"));

        var request = new UpdateHabitDisciplineStatusRequestDto
        {
            StatusHabitDisciplineIsActive = false,
        };

        var exception = await Assert.ThrowsAsync<ServerError>(
            () => _service.UpdateDisciplineStatusAsync(1, request));

        Assert.Equal(500, exception.HttpStatusCode);
    }
}
