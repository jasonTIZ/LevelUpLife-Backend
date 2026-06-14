using LevelUpLifeBackend.Data;
using LevelUpLifeBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace LevelUpLifeBackend.Repositories;

public class StreakLogRepository : IStreakLogRepository
{
    private readonly AppDbContext _context;

    public StreakLogRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<StreakLog?> GetByPlayerAndDateAsync(int playerUserId, DateOnly logDate)
    {
        return _context.StreakLogs
            .FirstOrDefaultAsync(s => s.PlayerUserId == playerUserId && s.LogDate == logDate);
    }

    public Task<StreakLog?> GetLastCompletionBeforeAsync(int playerUserId, DateOnly beforeDate)
    {
        return _context.StreakLogs
            .Where(s => s.PlayerUserId == playerUserId
                && s.LogDate < beforeDate
                && s.CompletionRecorded)
            .OrderByDescending(s => s.LogDate)
            .FirstOrDefaultAsync();
    }

    public Task<bool> HasProtectionInGapAsync(int playerUserId, DateOnly afterDate, DateOnly beforeDate)
    {
        return _context.StreakLogs.AnyAsync(s =>
            s.PlayerUserId == playerUserId
            && s.LogDate > afterDate
            && s.LogDate < beforeDate
            && s.ProtectionUsed);
    }

    public Task<int> CountProtectionsInMonthAsync(int playerUserId, int year, int month)
    {
        var start = new DateOnly(year, month, 1);
        var end = start.AddMonths(1);

        return _context.StreakLogs.CountAsync(s =>
            s.PlayerUserId == playerUserId
            && s.ProtectionUsed
            && s.LogDate >= start
            && s.LogDate < end);
    }

    public async Task AddAsync(StreakLog streakLog)
    {
        await _context.StreakLogs.AddAsync(streakLog);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(StreakLog streakLog)
    {
        _context.StreakLogs.Update(streakLog);
        await _context.SaveChangesAsync();
    }
}
