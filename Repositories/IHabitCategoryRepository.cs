using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.Repositories;

public interface IHabitCategoryRepository
{
    Task<HabitCategory?> GetByIdAsync(int id);

    Task<(
        IEnumerable<(HabitCategory Category, int HabitsCount)> HabitCategories,
        int TotalCount
    )> GetActiveHabitCategoriesPaginatedAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        int? userId = null
    );
}
