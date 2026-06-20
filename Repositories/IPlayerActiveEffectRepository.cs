using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.Repositories;

public interface IPlayerActiveEffectRepository
{
    Task<IEnumerable<PlayerActiveEffect>> GetActiveByPlayerAsync(int playerUserId);
    Task<PlayerActiveEffect?> GetActiveByInventoryIdAsync(int inventoryId);
    Task<PlayerActiveEffect> AddAsync(PlayerActiveEffect effect);
    Task UpdateAsync(PlayerActiveEffect effect);
}
