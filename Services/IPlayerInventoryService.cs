using LevelUpLifeBackend.DTOs.Responses;

namespace LevelUpLifeBackend.Services;

public interface IPlayerInventoryService
{
    Task<PlayerInventoryResponseDto> PurchaseAsync(int playerUserId, int rewardItemId);
    Task<IEnumerable<PlayerInventoryResponseDto>> GetInventoryAsync(int playerUserId);
}
