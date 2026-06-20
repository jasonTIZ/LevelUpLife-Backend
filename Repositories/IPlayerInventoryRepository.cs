using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.Repositories;

public interface IPlayerInventoryRepository
{
    Task<IEnumerable<PlayerInventory>> GetByPlayerIdAsync(int playerUserId);
}
