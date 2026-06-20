using LevelUpLifeBackend.DTOs.Responses;

namespace LevelUpLifeBackend.Services;

public interface IPlayerInventoryService
{
    Task<IEnumerable<PlayerInventoryItemResponseDto>> GetByPlayerIdAsync(int playerUserId);
}
