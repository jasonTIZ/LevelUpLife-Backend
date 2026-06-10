using LevelUpLifeBackend.Data;
using LevelUpLifeBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace LevelUpLifeBackend.Repositories;

public class TimerCriteriaRepository : ITimerCriteriaRepository
{
    private readonly AppDbContext _context;

    public TimerCriteriaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TimerCriteria?> GetByTaskIdAsync(int taskId)
    {
        return await _context.TimerCriteriaRecords
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.HabitTaskId == taskId);
    }
}
