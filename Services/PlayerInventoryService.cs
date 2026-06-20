using LevelUpLifeBackend.DTOs.Responses;
using LevelUpLifeBackend.Infrastructure.Errors;
using LevelUpLifeBackend.Models;
using LevelUpLifeBackend.Repositories;

namespace LevelUpLifeBackend.Services;

public class PlayerInventoryService : IPlayerInventoryService
{
    private readonly IPlayerInventoryRepository _inventoryRepository;
    private readonly IRewardItemRepository _rewardItemRepository;

    public PlayerInventoryService(
        IPlayerInventoryRepository inventoryRepository,
        IRewardItemRepository rewardItemRepository)
    {
        _inventoryRepository = inventoryRepository;
        _rewardItemRepository = rewardItemRepository;
    }

    public async Task<PlayerInventoryResponseDto> PurchaseAsync(int playerUserId, int rewardItemId)
    {
        var items = await _rewardItemRepository.GetFilteredAsync(null);
        var item = items.FirstOrDefault(i => i.Id == rewardItemId && i.IsActive);

        if (item is null)
            throw new NotFoundError(new ErrorResponse
            {
                Code = 404,
                Message = "Reward item not found.",
                Details = $"No active reward item with ID {rewardItemId} exists."
            });

        var existing = await _inventoryRepository.GetByPlayerAndItemAsync(playerUserId, rewardItemId);

        PlayerInventory result;

        if (existing is not null)
        {
            existing.Quantity += 1;
            result = await _inventoryRepository.UpdateAsync(existing);
        }
        else
        {
            var entry = new PlayerInventory
            {
                PlayerUserId = playerUserId,
                RewardItemId = rewardItemId,
                Quantity = 1,
                IsEquipped = false,
                AcquiredAt = DateTime.UtcNow,
            };
            result = await _inventoryRepository.AddAsync(entry);
        }

        return ToDto(result);
    }

    public async Task<IEnumerable<PlayerInventoryResponseDto>> GetInventoryAsync(int playerUserId)
    {
        var entries = await _inventoryRepository.GetByPlayerAsync(playerUserId);
        return entries.Select(ToDto);
    }

    private static PlayerInventoryResponseDto ToDto(PlayerInventory entry) => new()
    {
        Id = entry.Id,
        PlayerUserId = entry.PlayerUserId,
        RewardItemId = entry.RewardItemId,
        RewardItemName = entry.RewardItem?.Name ?? string.Empty,
        RewardItemTypeId = entry.RewardItem?.TypeId ?? 0,
        RewardItemTypeName = entry.RewardItem?.Type?.Name ?? string.Empty,
        CostGold = entry.RewardItem?.CostGold ?? 0,
        EffectValue = entry.RewardItem?.EffectValue,
        Quantity = entry.Quantity,
        IsEquipped = entry.IsEquipped,
        AcquiredAt = entry.AcquiredAt,
    };
}
