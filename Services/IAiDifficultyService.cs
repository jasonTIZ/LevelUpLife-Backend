using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.Services;

public record AiDifficultyResult(TaskDifficulty Difficulty, bool AiFailed);

public interface IAiDifficultyService
{
    Task<AiDifficultyResult> ClassifyAsync(
        string title,
        string? description,
        CancellationToken ct = default
    );
}
