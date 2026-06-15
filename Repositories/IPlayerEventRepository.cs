using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.Repositories;

public interface IPlayerEventRepository
{
    Task AddAsync(PlayerEvent playerEvent);
}
