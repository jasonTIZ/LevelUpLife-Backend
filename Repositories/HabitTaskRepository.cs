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

    public async Task<IEnumerable<EvidenceStorage>> GetEvidencesByTaskIdAsync(int taskId)
    {
        return await _context.EvidenceStorages
            .Where(e => e.HabitTaskId == taskId)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(int taskId)
    {
        return await _context.HabitTasks.AnyAsync(ht => ht.Id == taskId);
    }
}
