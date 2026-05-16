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

    public async Task<HabitDiscipline?> GetByIdAsync(int id)
    {
        return await _context.Disciplines
            .AsNoTracking()
            .Include(d => d.Category)
            .FirstOrDefaultAsync(d => d.Id == id);
    }
}
