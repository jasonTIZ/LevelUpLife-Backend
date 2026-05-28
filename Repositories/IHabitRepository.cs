using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.Repositories;

public interface IHabitRepository
{
    Task<Habit?> GetByIdAsync(int id, int? userId = null);
    Task<Habit> AddAsync(Habit habit);
    Task<(IEnumerable<Habit> Habits, int TotalCount)> GetActiveHabitsPaginatedAsync(
        int pageNumber,
        int pageSize,
        int userId
    );
    Task UpdateHabitAsync(Habit habit);
}
