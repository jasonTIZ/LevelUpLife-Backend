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
    private readonly HabitDisciplineService _service;

    public HabitDisciplineServiceTests()
    {
        _repositoryMock = new Mock<IHabitDisciplineRepository>();
        _service = new HabitDisciplineService(_repositoryMock.Object);
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
}
