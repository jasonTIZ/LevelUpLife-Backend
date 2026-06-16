using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.Repositories;

public interface IStreakLogRepository
{
    Task<StreakLog?> GetByPlayerAndDateAsync(int playerUserId, DateOnly logDate);
    Task<StreakLog?> GetLastCompletionBeforeAsync(int playerUserId, DateOnly beforeDate);
    Task<bool> HasProtectionInGapAsync(int playerUserId, DateOnly afterDate, DateOnly beforeDate);
    Task<int> CountProtectionsInMonthAsync(int playerUserId, int year, int month);
    Task AddAsync(StreakLog streakLog);
    Task UpdateAsync(StreakLog streakLog);
}
