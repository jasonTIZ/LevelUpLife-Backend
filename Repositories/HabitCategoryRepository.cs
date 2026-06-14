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
        IEnumerable<(HabitCategory Category, int HabitsCount)> HabitCategories,
        int TotalCount
    )> GetActiveHabitCategoriesPaginatedAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        int? userId = null
    )
    {
        var baseQuery = _context.HabitCategories.AsNoTracking().Where(h => h.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = $"%{search.Trim()}%";
            baseQuery = baseQuery.Where(h =>
                EF.Functions.ILike(h.Name, term) || EF.Functions.ILike(h.Description, term)
            );
        }

        var totalCount = await baseQuery.CountAsync();

        var habitCategories = await baseQuery
            .OrderBy(h => h.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new
            {
                Category = c,
                // Conteo de hábitos activos de la categoría que pertenecen al
                // usuario autenticado. Sin usuario, el conteo es 0.
                HabitsCount = userId == null
                    ? 0
                    : _context.Habits.Count(x =>
                        x.IsActive
                        && x.Discipline.Category.Id == c.Id
                        && x.User.Id == userId
                    ),
            })
            .ToListAsync();

        return (habitCategories.Select(x => (x.Category, x.HabitsCount)), totalCount);
    }
}
