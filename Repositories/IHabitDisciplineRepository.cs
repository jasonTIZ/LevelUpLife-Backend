using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.Repositories;

public interface IHabitDisciplineRepository
{
    Task<HabitDiscipline?> GetByIdAsync(int id);

    Task<HabitDiscipline> AddAsync(HabitDiscipline discipline);
}
