using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.Repositories;

public interface IHabitCategoryRepository
{
    Task<(
        IEnumerable<HabitCategory> HabitCategories,
        int TotalCount
    )> GetActiveHabitCategoriesPaginatedAsync(int pageNumber, int pageSize);
}
