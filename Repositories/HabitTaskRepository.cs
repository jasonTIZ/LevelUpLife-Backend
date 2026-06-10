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

    public Task<HabitTask?> GetTrackedByIdForUserAsync(int taskId, int userId)
    {
        return _context.HabitTasks
            .Include(t => t.Habit!)
                .ThenInclude(h => h.User)
            .Include(t => t.Habit!)
                .ThenInclude(h => h.Discipline)
            .Include(t => t.RepetitionCriteria)
            .FirstOrDefaultAsync(t => t.Id == taskId && t.Habit!.User.Id == userId);
    }

    public async Task UpdateAsync(HabitTask task)
    {
        await _context.SaveChangesAsync();
    }

    public async Task UpdateWithRepetitionCriteriaAsync(HabitTask task)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            if (task.RepetitionCriteria is not null && task.RepetitionCriteria.Id == 0)
            {
                await _context.RepetitionCriteriaRecords.AddAsync(task.RepetitionCriteria);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public Task<HabitTask?> GetByIdWithCriteriaAsync(int id)
    {
        return _context.HabitTasks
            .AsNoTracking()
            .Include(t => t.RepetitionCriteria)
            .Include(t => t.HabitDiscipline)
                .ThenInclude(d => d!.Category)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<EvidenceStorage>> GetEvidencesByTaskIdAsync(int taskId)
    {
        return await _context.EvidenceStorages
            .Where(e => e.HabitTaskId == taskId)
            .ToListAsync();
    }

    public async Task<EvidenceStorage?> GetEvidenceByIdAsync(int taskId, int id)
    {
        return await _context.EvidenceStorages
            .FirstOrDefaultAsync(e => e.HabitTaskId == taskId && e.Id == id);
    }

    public async Task<bool> ExistsAsync(int taskId)
    {
        return await _context.HabitTasks.AnyAsync(ht => ht.Id == taskId);
    }
}
