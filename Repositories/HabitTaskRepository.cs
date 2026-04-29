using LevelUpLifeBackend.Data;
using LevelUpLifeBackend.Models;

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
}
