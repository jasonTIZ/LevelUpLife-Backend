using System.Text.Json;
using LevelUpLifeBackend.Data;
using LevelUpLifeBackend.DTOs.Responses;
using LevelUpLifeBackend.Infrastructure;
using LevelUpLifeBackend.Infrastructure.Errors;
using LevelUpLifeBackend.Models;
using LevelUpLifeBackend.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LevelUpLifeBackend.Services;

public class PlayerInventoryService : IPlayerInventoryService
{
    private readonly AppDbContext _context;
    private readonly IRewardItemRepository _rewardItemRepository;
    private readonly IPlayerActiveEffectRepository _activeEffectRepository;
    private readonly IPlayerEffectService _playerEffectService;
    private readonly ILevelProgressService _levelProgressService;

    public PlayerInventoryService(
        AppDbContext context,
        IRewardItemRepository rewardItemRepository,
        IPlayerActiveEffectRepository activeEffectRepository,
        IPlayerEffectService playerEffectService,
        ILevelProgressService levelProgressService)
    {
        _context = context;
        _rewardItemRepository = rewardItemRepository;
        _activeEffectRepository = activeEffectRepository;
        _playerEffectService = playerEffectService;
        _levelProgressService = levelProgressService;
    }

    public async Task<PurchaseRewardItemResponseDto> PurchaseAsync(int playerUserId, int rewardItemId)
    {
        var item = await _rewardItemRepository.GetActiveByIdAsync(rewardItemId);
        if (item is null)
        {
            throw new NotFoundError(new ErrorResponse
            {
                Code = 404,
                Message = "Reward item not found.",
                Details = $"No active reward item with ID {rewardItemId} exists.",
            });
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var player = await _context.PlayerUsers
                .FirstOrDefaultAsync(u => u.Id == playerUserId && u.IsActive);

            if (player is null)
            {
                throw new NotFoundError(new ErrorResponse
                {
                    Code = 404,
                    Message = "Player not found",
                    Details = $"Player with id {playerUserId} was not found or is inactive.",
                });
            }

            if (player.Gold < item.CostGold)
            {
                throw new InventoryError(
                    new ErrorResponse
                    {
                        Code = 400,
                        Message = "Insufficient gold.",
                        Details = $"This item costs {item.CostGold} gold but you only have {player.Gold}.",
                    },
                    InventoryFailureKind.InsufficientGold);
            }

            player.Gold -= item.CostGold;

            var existing = await _context.PlayerInventories
                .Include(i => i.RewardItem)
                    .ThenInclude(r => r!.Type)
                .FirstOrDefaultAsync(i => i.PlayerUserId == playerUserId && i.RewardItemId == rewardItemId);

            PlayerInventory result;
            if (existing is not null)
            {
                existing.Quantity += 1;
                result = existing;
            }
            else
            {
                result = new PlayerInventory
                {
                    PlayerUserId = playerUserId,
                    RewardItemId = rewardItemId,
                    Quantity = 1,
                    IsEquipped = false,
                    AcquiredAt = DateTime.UtcNow,
                    RewardItem = item,
                };
                await _context.PlayerInventories.AddAsync(result);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            if (result.RewardItem is null)
            {
                await _context.Entry(result).Reference(i => i.RewardItem).LoadAsync();
                if (result.RewardItem is not null)
                    await _context.Entry(result.RewardItem).Reference(r => r.Type).LoadAsync();
            }

            return new PurchaseRewardItemResponseDto
            {
                Inventory = ToInventoryDto(result),
                RemainingGold = player.Gold,
            };
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<ActivateInventoryItemResponseDto> ActivateAsync(int playerUserId, int inventoryId)
    {
        await _playerEffectService.DeactivateExpiredEffectsAsync(playerUserId);

        var entry = await _context.PlayerInventories
            .Include(i => i.RewardItem)
                .ThenInclude(r => r!.Type)
            .FirstOrDefaultAsync(i => i.Id == inventoryId && i.PlayerUserId == playerUserId);

        if (entry is null)
        {
            throw new NotFoundError(new ErrorResponse
            {
                Code = 404,
                Message = "Inventory item not found.",
                Details = $"No inventory entry with ID {inventoryId} exists for this player.",
            });
        }

        if (entry.Quantity <= 0)
        {
            throw new InventoryError(
                new ErrorResponse
                {
                    Code = 400,
                    Message = "Item not activatable.",
                    Details = "You do not have any units of this item left.",
                },
                InventoryFailureKind.ItemNotActivatable);
        }

        var item = entry.RewardItem;
        if (item is null || !item.IsActive)
        {
            throw new NotFoundError(new ErrorResponse
            {
                Code = 404,
                Message = "Reward item not found.",
                Details = "The linked reward item is missing or inactive.",
            });
        }

        var existingEffect = await _activeEffectRepository.GetActiveByInventoryIdAsync(inventoryId);
        if (existingEffect is not null)
        {
            throw new InventoryError(
                new ErrorResponse
                {
                    Code = 400,
                    Message = "Effect already active.",
                    Details = "This inventory item already has an active effect.",
                },
                InventoryFailureKind.EffectAlreadyActive);
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var response = new ActivateInventoryItemResponseDto();

            if (item.TypeId == RewardItemTypeIds.Recovery)
            {
                response.RecoveryMessage = await ApplyRecoveryEffectAsync(playerUserId, item);
                entry.Quantity -= 1;
                entry.IsEquipped = false;
            }
            else
            {
                var effect = CreateActiveEffect(playerUserId, entry, item);
                await _context.PlayerActiveEffects.AddAsync(effect);
                entry.Quantity -= 1;
                entry.IsEquipped = true;
                response.ActiveEffect = ToEffectDto(effect, item);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            if (response.ActiveEffect is not null && response.ActiveEffect.RewardItemName == string.Empty)
                response.ActiveEffect = ToEffectDto(
                    await _context.PlayerActiveEffects
                        .Include(e => e.RewardItem)
                            .ThenInclude(r => r!.Type)
                        .OrderByDescending(e => e.Id)
                        .FirstAsync(e => e.InventoryId == inventoryId && e.IsActive),
                    item);

            response.Inventory = ToInventoryDto(entry);
            return response;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<PlayerInventoryListResponseDto> GetInventoryAsync(int playerUserId)
    {
        await _playerEffectService.DeactivateExpiredEffectsAsync(playerUserId);

        var entries = await _context.PlayerInventories
            .AsNoTracking()
            .Include(i => i.RewardItem)
                .ThenInclude(r => r!.Type)
            .Where(i => i.PlayerUserId == playerUserId)
            .OrderBy(i => i.AcquiredAt)
            .ToListAsync();

        var activeEffects = await _context.PlayerActiveEffects
            .AsNoTracking()
            .Include(e => e.RewardItem)
                .ThenInclude(r => r!.Type)
            .Where(e => e.PlayerUserId == playerUserId && e.IsActive)
            .OrderByDescending(e => e.ActivatedAt)
            .ToListAsync();

        return new PlayerInventoryListResponseDto
        {
            Items = entries.Select(ToInventoryDto),
            ActiveEffects = activeEffects.Select(e => ToEffectDto(e, e.RewardItem!)),
        };
    }

    private static PlayerActiveEffect CreateActiveEffect(int playerUserId, PlayerInventory entry, RewardItem item)
    {
        var now = DateTime.UtcNow;
        DateTime? expiresAt = null;
        int? remainingCharges = null;

        switch (item.TypeId)
        {
            case RewardItemTypeIds.StreakProtection:
                remainingCharges = item.EffectValue.HasValue
                    ? (int)item.EffectValue.Value
                    : 1;
                break;
            case RewardItemTypeIds.XpBoost:
            case RewardItemTypeIds.HabitExpansion:
                if (item.DurationDays.HasValue && item.DurationDays.Value > 0)
                    expiresAt = now.AddDays(item.DurationDays.Value);
                break;
        }

        return new PlayerActiveEffect
        {
            PlayerUserId = playerUserId,
            InventoryId = entry.Id,
            RewardItemId = item.Id,
            RewardItemTypeId = item.TypeId,
            EffectValue = item.EffectValue,
            RemainingCharges = remainingCharges,
            ActivatedAt = now,
            ExpiresAt = expiresAt,
            IsActive = true,
            RewardItem = item,
        };
    }

    private async Task<string> ApplyRecoveryEffectAsync(int playerUserId, RewardItem item)
    {
        var player = await _context.PlayerUsers
            .FirstOrDefaultAsync(u => u.Id == playerUserId && u.IsActive);

        if (player is null)
        {
            throw new NotFoundError(new ErrorResponse
            {
                Code = 404,
                Message = "Player not found",
                Details = $"Player with id {playerUserId} was not found or is inactive.",
            });
        }

        if (IsReviveStreakItem(item))
            return await ReviveStreakAsync(player);

        if (IsErasePenaltyItem(item))
            return await EraseLastPenaltyAsync(player);

        throw new InventoryError(
            new ErrorResponse
            {
                Code = 400,
                Message = "Item not activatable.",
                Details = "This recovery item is not supported.",
            },
            InventoryFailureKind.ItemNotActivatable);
    }

    private async Task<string> ReviveStreakAsync(PlayerUser player)
    {
        var lastReset = await _context.PlayerEvents
            .Where(e => e.PlayerUserId == player.Id && e.EventType == PlayerEventType.STREAK_RESET)
            .OrderByDescending(e => e.CreatedAt)
            .FirstOrDefaultAsync();

        if (lastReset?.PayloadJson is null
            || !TryReadIntProperty(lastReset.PayloadJson, "previousStreak", out var previousStreak)
            || previousStreak <= 0)
        {
            throw new InventoryError(
                new ErrorResponse
                {
                    Code = 400,
                    Message = "No recovery target.",
                    Details = "There is no recent streak loss to restore.",
                },
                InventoryFailureKind.NoRecoveryTarget);
        }

        player.DaysStreak = previousStreak;
        return $"Streak restored to {previousStreak} days.";
    }

    private async Task<string> EraseLastPenaltyAsync(PlayerUser player)
    {
        var lastPenalty = await _context.PlayerEvents
            .Where(e => e.PlayerUserId == player.Id && e.EventType == PlayerEventType.XP_PENALTY)
            .OrderByDescending(e => e.CreatedAt)
            .FirstOrDefaultAsync();

        if (lastPenalty?.PayloadJson is null
            || !TryReadIntProperty(lastPenalty.PayloadJson, "amount", out var amount)
            || amount <= 0)
        {
            throw new InventoryError(
                new ErrorResponse
                {
                    Code = 400,
                    Message = "No recovery target.",
                    Details = "There is no XP penalty available to erase.",
                },
                InventoryFailureKind.NoRecoveryTarget);
        }

        var alreadyReverted = await _context.PlayerEvents.AnyAsync(e =>
            e.PlayerUserId == player.Id
            && e.EventType == PlayerEventType.XP_PENALTY_REVERTED
            && e.CreatedAt > lastPenalty.CreatedAt);

        if (alreadyReverted)
        {
            throw new InventoryError(
                new ErrorResponse
                {
                    Code = 400,
                    Message = "No recovery target.",
                    Details = "The last XP penalty has already been erased.",
                },
                InventoryFailureKind.NoRecoveryTarget);
        }

        player.ExperiencePoints += amount;
        player.Level = _levelProgressService.CalculateLevel(player.ExperiencePoints);

        await _context.PlayerEvents.AddAsync(new PlayerEvent
        {
            PlayerUserId = player.Id,
            EventType = PlayerEventType.XP_PENALTY_REVERTED,
            PayloadJson = $"{{\"amount\":{amount},\"xpAfter\":{player.ExperiencePoints}}}",
            CreatedAt = DateTime.UtcNow,
        });

        return $"Restored {amount} XP.";
    }

    private static bool IsReviveStreakItem(RewardItem item) =>
        item.Name.Contains("Revivir", StringComparison.OrdinalIgnoreCase);

    private static bool IsErasePenaltyItem(RewardItem item) =>
        item.Name.Contains("Borrador", StringComparison.OrdinalIgnoreCase);

    private static bool TryReadIntProperty(string json, string propertyName, out int value)
    {
        value = 0;
        try
        {
            using var document = JsonDocument.Parse(json);
            if (document.RootElement.TryGetProperty(propertyName, out var property)
                && property.TryGetInt32(out value))
            {
                return true;
            }
        }
        catch (JsonException)
        {
        }

        return false;
    }

    private static PlayerInventoryResponseDto ToInventoryDto(PlayerInventory entry) => new()
    {
        Id = entry.Id,
        PlayerUserId = entry.PlayerUserId,
        RewardItemId = entry.RewardItemId,
        RewardItemName = entry.RewardItem?.Name ?? string.Empty,
        RewardItemTypeId = entry.RewardItem?.TypeId ?? 0,
        RewardItemTypeName = entry.RewardItem?.Type?.Name ?? string.Empty,
        CostGold = entry.RewardItem?.CostGold ?? 0,
        EffectValue = entry.RewardItem?.EffectValue,
        DurationDays = entry.RewardItem?.DurationDays,
        Quantity = entry.Quantity,
        IsEquipped = entry.IsEquipped,
        AcquiredAt = entry.AcquiredAt,
    };

    private static PlayerActiveEffectResponseDto ToEffectDto(PlayerActiveEffect effect, RewardItem item) => new()
    {
        Id = effect.Id,
        InventoryId = effect.InventoryId,
        RewardItemId = effect.RewardItemId,
        RewardItemName = item.Name,
        RewardItemTypeId = effect.RewardItemTypeId,
        RewardItemTypeName = item.Type?.Name ?? string.Empty,
        EffectValue = effect.EffectValue,
        RemainingCharges = effect.RemainingCharges,
        ActivatedAt = effect.ActivatedAt,
        ExpiresAt = effect.ExpiresAt,
        IsActive = effect.IsActive,
    };
}
