using LevelUpLifeBackend.DTOs.Responses;

namespace LevelUpLifeBackend.Services;

public interface IPlayerInventoryService
{
    Task<PurchaseRewardItemResponseDto> PurchaseAsync(int playerUserId, int rewardItemId);
    Task<ActivateInventoryItemResponseDto> ActivateAsync(int playerUserId, int inventoryId);
    Task<PlayerInventoryListResponseDto> GetInventoryAsync(int playerUserId);
}
