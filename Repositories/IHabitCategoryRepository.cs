using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.Repositories;

public interface IHabitCategoryRepository
{
    Task<HabitCategory?> GetByIdAsync(int id);

    Task<(
        IEnumerable<HabitCategory> HabitCategories,
        int TotalCount
    )> GetActiveHabitCategoriesPaginatedAsync(int pageNumber, int pageSize);
}
