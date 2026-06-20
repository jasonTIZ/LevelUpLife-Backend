using LevelUpLifeBackend.DTOs.Responses;
using LevelUpLifeBackend.Repositories;

namespace LevelUpLifeBackend.Services;

public class PlayerInventoryService : IPlayerInventoryService
{
    private readonly IPlayerInventoryRepository _playerInventoryRepository;

    public PlayerInventoryService(IPlayerInventoryRepository playerInventoryRepository)
    {
        _playerInventoryRepository = playerInventoryRepository;
    }

    public async Task<IEnumerable<PlayerInventoryItemResponseDto>> GetByPlayerIdAsync(int playerUserId)
    {
        var items = await _playerInventoryRepository.GetByPlayerIdAsync(playerUserId);

        return items.Select(i => new PlayerInventoryItemResponseDto
        {
            Id = i.Id,
            RewardItemId = i.RewardItemId,
            ItemName = i.RewardItem?.Name ?? string.Empty,
            ItemDescription = i.RewardItem?.Description,
            TypeId = i.RewardItem?.TypeId ?? 0,
            TypeName = i.RewardItem?.Type?.Name ?? string.Empty,
            Quantity = i.Quantity,
            IsEquipped = i.IsEquipped,
            AcquiredAt = i.AcquiredAt,
            CostGold = i.RewardItem?.CostGold ?? 0,
            EffectValue = i.RewardItem?.EffectValue,
        });
    }
}
