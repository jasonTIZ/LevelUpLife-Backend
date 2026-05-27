using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.Repositories;

public interface IHabitRepository
{
    Task<Habit?> GetByIdAsync(int id);
    Task<Habit?> GetByIdForUserAsync(int id, int userId);
    Task<Habit> AddAsync(Habit habit);
    Task<(IEnumerable<Habit> Habits, int TotalCount)> GetActiveHabitsPaginatedAsync(
        int pageNumber,
        int pageSize,
        int userId
    );
    Task UpdateHabitAsync(Habit habit);
}
