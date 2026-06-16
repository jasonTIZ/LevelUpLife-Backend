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

    public async Task<HabitDiscipline?> GetByIdForUpdateAsync(int id)
    {
        return await _context.Disciplines.FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<HabitDiscipline> AddAsync(HabitDiscipline discipline)
    {
        _context.Entry(discipline.Category).State = EntityState.Unchanged;
        await _context.Disciplines.AddAsync(discipline);
        await _context.SaveChangesAsync();
        return discipline;
    }

    public async Task<HabitDiscipline> UpdateAsync(HabitDiscipline discipline, int categoryId)
    {
        _context.Entry(discipline).Property("CategoryId").CurrentValue = categoryId;
        await _context.SaveChangesAsync();
        return discipline;
    }
}
