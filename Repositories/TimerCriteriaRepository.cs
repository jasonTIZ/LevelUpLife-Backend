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
            .FirstOrDefaultAsync(c => c.HabitTaskId == taskId);
    }

    public async Task<TimerCriteria> UpdateAsync(TimerCriteria criteria)
    {
        _context.Update(criteria);
        await _context.SaveChangesAsync();
        return criteria;
    }
}