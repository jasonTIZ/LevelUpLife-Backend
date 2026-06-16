using LevelUpLifeBackend.Data;
using LevelUpLifeBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace LevelUpLifeBackend.Repositories;

public class HabitDisciplineRepository : IHabitDisciplineRepository
{
    private readonly AppDbContext _context;

    public HabitDisciplineRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<HabitDiscipline>> GetAllAsync()
    {
        return await _context.Disciplines
            .AsNoTracking()
            .Include(d => d.Category)
            .ToListAsync();
    }

    public async Task<HabitDiscipline?> GetByIdAsync(int id)
    {
        return await _context.Disciplines
            .AsNoTracking()
            .Include(d => d.Category)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<HabitDiscipline> AddAsync(HabitDiscipline discipline)
    {
        _context.Entry(discipline.Category).State = EntityState.Unchanged;
        await _context.Disciplines.AddAsync(discipline);
        await _context.SaveChangesAsync();
        return discipline;
    }

    public async Task<HabitDiscipline?> GetTrackedByIdAsync(int id)
    {
        return await _context.Disciplines
            .Include(d => d.Category)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task UpdateAsync(HabitDiscipline discipline)
    {
        _context.Disciplines.Update(discipline);
        await _context.SaveChangesAsync();
    }
}
