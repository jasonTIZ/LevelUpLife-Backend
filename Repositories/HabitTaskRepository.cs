using LevelUpLifeBackend.Data;
using LevelUpLifeBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace LevelUpLifeBackend.Repositories;

public class HabitTaskRepository : IHabitTaskRepository
{
    private readonly AppDbContext _context;

    public HabitTaskRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<HabitTask> AddAsync(HabitTask task)
    {
        await _context.HabitTasks.AddAsync(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public Task<bool> ExistsActiveByHabitIdAsync(int habitId)
    {
        return _context.HabitTasks.AsNoTracking().AnyAsync(t => t.HabitId == habitId && t.IsActive);
    }

    public Task<HabitTask?> GetByIdWithCriteriaAsync(int id)
    {
        return _context.HabitTasks
            .AsNoTracking()
            .Include(t => t.RepetitionCriteria)
            .FirstOrDefaultAsync(t => t.Id == id);
    }
}
