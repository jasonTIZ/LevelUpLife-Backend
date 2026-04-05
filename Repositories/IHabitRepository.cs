using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.Repositories;

public interface IHabitRepository
{
    Task<Habit?> GetByIdAsync(int id);
    Task<Habit> AddAsync(Habit habit);
}
