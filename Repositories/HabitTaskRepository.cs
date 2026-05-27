using LevelUpLifeBackend.Data;
using LevelUpLifeBackend.DTOs.Requests;
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

    public Task<bool> HabitBelongsToUserAsync(int habitId, int userId)
    {
        return _context.Habits
            .AsNoTracking()
            .AnyAsync(h => h.Id == habitId && EF.Property<int>(h, "UserId") == userId);
    }

    public async Task<(IReadOnlyList<HabitTask> Items, int Total)> ListByHabitAndUserAsync(
        int habitId,
        int userId,
        HabitTaskListQueryDto filter
    )
    {
        var query = _context.HabitTasks
            .AsNoTracking()
            .Where(t =>
                t.HabitId == habitId
                && _context.Habits.Any(h => h.Id == habitId && EF.Property<int>(h, "UserId") == userId)
            );

        if (filter.DisciplineId.HasValue)
            query = query.Where(t => t.HabitDisciplineId == filter.DisciplineId.Value);

        if (filter.Difficulty.HasValue)
            query = query.Where(t => t.Difficulty == filter.Difficulty.Value);

        if (filter.Frequency.HasValue)
            query = query.Where(t => t.Frequency == filter.Frequency.Value);

        if (filter.IsActive.HasValue)
            query = query.Where(t => t.IsActive == filter.IsActive.Value);

        var total = await query.CountAsync();

        var items = await query
            .OrderBy(t => t.Id)
            .Skip(filter.Page * filter.Size)
            .Take(filter.Size)
            .ToListAsync();

        return (items, total);
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
