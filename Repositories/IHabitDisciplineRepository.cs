using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.Repositories;

public interface IHabitDisciplineRepository
{
    Task<IEnumerable<HabitDiscipline>> GetAllAsync();

    Task<HabitDiscipline?> GetByIdAsync(int id);

    Task<HabitDiscipline> AddAsync(HabitDiscipline discipline);

    Task<HabitDiscipline?> GetTrackedByIdAsync(int id);

    Task UpdateAsync(HabitDiscipline discipline);
}
