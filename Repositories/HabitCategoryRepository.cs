using LevelUpLifeBackend.Data;
using LevelUpLifeBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace LevelUpLifeBackend.Repositories;

public class HabitCategoryRepository : IHabitCategoryRepository
{
    private readonly AppDbContext _context;

    public HabitCategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<HabitCategory?> GetByIdAsync(int id)
    {
        return await _context.HabitCategories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<(
        IEnumerable<HabitCategory> HabitCategories,
        int TotalCount
    )> GetActiveHabitCategoriesPaginatedAsync(int pageNumber, int pageSize)
    {
        var baseQuery = _context.HabitCategories.AsNoTracking().Where(h => h.IsActive);
        var totalCount = await baseQuery.CountAsync();

        var habitCategories = await baseQuery
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (habitCategories, totalCount);
    }
}
