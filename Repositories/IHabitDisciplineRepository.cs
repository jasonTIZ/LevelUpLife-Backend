using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.Repositories;

public interface IHabitDisciplineRepository
{
    Task<HabitDiscipline?> GetByIdAsync(int id);

    Task<HabitDiscipline?> GetByIdForUpdateAsync(int id);

    Task<HabitDiscipline> AddAsync(HabitDiscipline discipline);

    Task<HabitDiscipline> UpdateAsync(HabitDiscipline discipline, int categoryId);
}
