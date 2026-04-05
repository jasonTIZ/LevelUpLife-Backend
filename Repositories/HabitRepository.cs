using Microsoft.EntityFrameworkCore;
using LevelUpLifeBackend.Data;
using LevelUpLifeBackend.Models;

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

    public async Task<Habit?> GetByIdAsync(int id){
       return await _context.Habits.AsNoTracking().FirstOrDefaultAsync(habit => habit.Id == id);
    }
}
