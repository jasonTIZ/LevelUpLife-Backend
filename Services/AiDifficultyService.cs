using LevelUpLifeBackend.Infrastructure.Ai;
using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.Services;

public class AiDifficultyService : IAiDifficultyService
{
    private readonly IAiProvider _aiProvider;

    public AiDifficultyService(IAiProvider aiProvider)
    {
        _aiProvider = aiProvider;
    }

    private const string SystemPrompt =
        "Eres un clasificador de dificultad para tareas de hábitos personales. "
        + "Según el esfuerzo físico o mental que implica la tarea, responde ÚNICAMENTE con una sola palabra "
        + "en mayúsculas: EASY, MEDIUM, HARD o EPIC. No agregues explicaciones ni puntuación.";

    public async Task<AiDifficultyResult> ClassifyAsync(
        string title,
        string? description,
        CancellationToken ct = default
    )
    {
        try
        {
            var message =
                $"Título: {title}\nDescripción: {(string.IsNullOrWhiteSpace(description) ? "(sin descripción)" : description)}";

            var (reply, _) = await _aiProvider.CompleteAsync(
                userId: $"difficulty-{Guid.NewGuid():N}",
                message: message,
                systemPrompt: SystemPrompt,
                model: null,
                ct: ct
            );

            var parsed = ParseDifficulty(reply);
            return parsed is null
                ? new AiDifficultyResult(TaskDifficulty.MEDIUM, true)
                : new AiDifficultyResult(parsed.Value, false);
        }
        catch
        {
            return new AiDifficultyResult(TaskDifficulty.MEDIUM, true);
        }
    }

    private static TaskDifficulty? ParseDifficulty(string? reply)
    {
        if (string.IsNullOrWhiteSpace(reply))
        {
            return null;
        }

        var text = reply.ToUpperInvariant();
        foreach (var difficulty in Enum.GetValues<TaskDifficulty>())
        {
            if (text.Contains(difficulty.ToString()))
            {
                return difficulty;
            }
        }

        return null;
    }
}
