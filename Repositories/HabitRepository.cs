using LevelUpLifeBackend.Data;
using LevelUpLifeBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace LevelUpLifeBackend.Repositories;

public class HabitRepository : IHabitRepository
{
    private readonly AppDbContext _context;

    public HabitRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Habit> AddAsync(Habit habit)
    {
        _context.Entry(habit.Discipline).State = EntityState.Unchanged;
        _context.Entry(habit.User).State = EntityState.Unchanged;
        await _context.Habits.AddAsync(habit);
        await _context.SaveChangesAsync();
        return habit;
    }

    public async Task<Habit?> GetByIdAsync(int id, int userId)
    {
        return await _context.Habits
            .AsNoTracking()
            .Include(h => h.Discipline)
                .ThenInclude(d => d.Category)
            .Include(h => h.User)
            .Include(h => h.Tasks)
                .ThenInclude(t => t.RepetitionCriteria)
            .FirstOrDefaultAsync(h => h.Id == id && h.User.Id == userId);
    }

    public async Task<(IEnumerable<Habit> Habits, int TotalCount)> GetActiveHabitsPaginatedAsync(
        int pageNumber,
        int pageSize,
        int userId
    )
    {
        var baseQuery = _context.Habits.AsNoTracking().Where(h => h.IsActive && h.User.Id == userId);

        var totalCount = await baseQuery.CountAsync();

        var habits = await baseQuery
            .Include(h => h.Discipline)
                .ThenInclude(d => d.Category)
            .Include(h => h.User)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (habits, totalCount);
    }

    public async Task UpdateHabitAsync(Habit habit)
    {
        _context.Entry(habit.Discipline).State = EntityState.Unchanged;
        _context.Entry(habit.Discipline.Category).State = EntityState.Unchanged;
        _context.Entry(habit.User).State = EntityState.Unchanged;
        _context.Habits.Update(habit);
        await _context.SaveChangesAsync();
    }
}
