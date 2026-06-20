using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.Repositories;

public interface IPlayerInventoryRepository
{
    Task<PlayerInventory?> GetByIdForPlayerAsync(int inventoryId, int playerUserId);
    Task<PlayerInventory?> GetByPlayerAndItemAsync(int playerUserId, int rewardItemId);
    Task<PlayerInventory> AddAsync(PlayerInventory entry);
    Task<PlayerInventory> UpdateAsync(PlayerInventory entry);
    Task<IEnumerable<PlayerInventory>> GetByPlayerAsync(int playerUserId);
}
