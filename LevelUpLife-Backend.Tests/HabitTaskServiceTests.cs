using LevelUpLifeBackend.DTOs.Responses;
using LevelUpLifeBackend.Infrastructure.Errors;
using LevelUpLifeBackend.Models;
using LevelUpLifeBackend.Repositories;
using LevelUpLifeBackend.Services;
using Moq;
using Xunit;

namespace LevelUpLife_Backend.Tests;

public class HabitTaskServiceTests
{
    private readonly Mock<IHabitTaskRepository> _habitTaskRepositoryMock;
    private readonly HabitTaskService _habitTaskService;

    public HabitTaskServiceTests()
    {
        _habitTaskRepositoryMock = new Mock<IHabitTaskRepository>();
        _habitTaskService = new HabitTaskService(_habitTaskRepositoryMock.Object);
    }

    [Fact]
    public async Task GetEvidencesByTaskIdAsync_WhenTaskExists_ReturnsEvidences()
    {
        // Arrange
        int taskId = 1;
        var evidences = new List<EvidenceStorage>
        {
            new EvidenceStorage
            {
                Id = 1,
                HabitTaskId = taskId,
                EvidencePathUrl = "http://example.com/evidence1.jpg",
                UploadedAt = DateTime.UtcNow
            },
            new EvidenceStorage
            {
                Id = 2,
                HabitTaskId = taskId,
                HealthDataJson = "{\"steps\": 1000}",
                UploadedAt = DateTime.UtcNow
            }
        };

        _habitTaskRepositoryMock.Setup(repo => repo.ExistsAsync(taskId)).ReturnsAsync(true);
        _habitTaskRepositoryMock.Setup(repo => repo.GetEvidencesByTaskIdAsync(taskId)).ReturnsAsync(evidences);

        // Act
        var result = await _habitTaskService.GetEvidencesByTaskIdAsync(taskId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        var first = result.First();
        Assert.Equal(1, first.Id);
        Assert.Equal("http://example.com/evidence1.jpg", first.EvidencePathUrl);
    }

    [Fact]
    public async Task GetEvidencesByTaskIdAsync_WhenTaskDoesNotExist_ThrowsNotFoundError()
    {
        // Arrange
        int taskId = 1;
        _habitTaskRepositoryMock.Setup(repo => repo.ExistsAsync(taskId)).ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundError>(() => _habitTaskService.GetEvidencesByTaskIdAsync(taskId));
        Assert.Equal(404, exception.HttpStatusCode);
    }

    [Fact]
    public async Task GetEvidenceByIdAsync_WhenEvidenceExists_ReturnsEvidence()
    {
        // Arrange
        int taskId = 1;
        int id = 10;
        var evidence = new EvidenceStorage
        {
            Id = id,
            HabitTaskId = taskId,
            EvidencePathUrl = "http://example.com/evidence.jpg",
            UploadedAt = DateTime.UtcNow
        };

        _habitTaskRepositoryMock.Setup(repo => repo.ExistsAsync(taskId)).ReturnsAsync(true);
        _habitTaskRepositoryMock.Setup(repo => repo.GetEvidenceByIdAsync(taskId, id)).ReturnsAsync(evidence);

        // Act
        var result = await _habitTaskService.GetEvidenceByIdAsync(taskId, id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal(taskId, result.HabitTaskId);
    }

    [Fact]
    public async Task GetEvidenceByIdAsync_WhenTaskDoesNotExist_ThrowsNotFoundError()
    {
        // Arrange
        int taskId = 1;
        _habitTaskRepositoryMock.Setup(repo => repo.ExistsAsync(taskId)).ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundError>(() => _habitTaskService.GetEvidenceByIdAsync(taskId, 10));
        Assert.Equal(404, exception.HttpStatusCode);
    }

    [Fact]
    public async Task GetEvidenceByIdAsync_WhenEvidenceDoesNotExist_ThrowsNotFoundError()
    {
        // Arrange
        int taskId = 1;
        int id = 10;
        _habitTaskRepositoryMock.Setup(repo => repo.ExistsAsync(taskId)).ReturnsAsync(true);
        _habitTaskRepositoryMock.Setup(repo => repo.GetEvidenceByIdAsync(taskId, id)).ReturnsAsync((EvidenceStorage)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundError>(() => _habitTaskService.GetEvidenceByIdAsync(taskId, id));
        Assert.Equal(404, exception.HttpStatusCode);
    }

    [Fact]
    public async Task GetEvidenceByIdAsync_WhenRepositoryThrowsException_ThrowsServerError()
    {
        // Arrange
        int taskId = 1;
        _habitTaskRepositoryMock.Setup(repo => repo.ExistsAsync(taskId)).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ServerError>(() => _habitTaskService.GetEvidenceByIdAsync(taskId, 10));
        Assert.Equal(500, exception.HttpStatusCode);
    }
}
