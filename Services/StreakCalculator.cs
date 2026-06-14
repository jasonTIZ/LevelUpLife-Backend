using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.Services;

public static class StreakCalculator
{
    /// <summary>
    /// Calcula el conteo de racha para una fecha de completion.
    /// Anti-abuso: solo usa el último log real anterior a completionDate (sin backfill).
    /// Protección: si hay al menos un día protegido en el hueco, la racha continúa (+1) en lugar de reiniciarse.
    /// </summary>
    public static (int NewStreakCount, bool WasReset) ComputeStreakCount(
        StreakLog? lastCompletionLog,
        DateOnly completionDate,
        bool gapHasProtection
    )
    {
        if (lastCompletionLog is null)
        {
            return (1, false);
        }

        var dayGap = completionDate.DayNumber - lastCompletionLog.LogDate.DayNumber;

        if (dayGap <= 0)
        {
            return (Math.Max(1, lastCompletionLog.StreakCount), false);
        }

        if (dayGap == 1)
        {
            return (lastCompletionLog.StreakCount + 1, false);
        }

        if (gapHasProtection)
        {
            return (lastCompletionLog.StreakCount + 1, false);
        }

        return (1, true);
    }
}
