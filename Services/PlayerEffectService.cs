using System.Text.Json;
using LevelUpLifeBackend.Data;
using LevelUpLifeBackend.Infrastructure;
using LevelUpLifeBackend.Models;
using LevelUpLifeBackend.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LevelUpLifeBackend.Services;

public interface IPlayerEffectService
{
    Task DeactivateExpiredEffectsAsync(int playerUserId);
    Task<decimal> GetActiveXpMultiplierAsync(int playerUserId);
    Task<bool> TryConsumeStreakShieldAsync(int playerUserId, int missedDays);
    int CalculateXpPenalty(int currentExperiencePoints);
}

public class PlayerEffectService : IPlayerEffectService
{
    private readonly AppDbContext _context;

    public PlayerEffectService(AppDbContext context)
    {
        _context = context;
    }

    public async Task DeactivateExpiredEffectsAsync(int playerUserId)
    {
        var now = DateTime.UtcNow;
        var effects = await _context.PlayerActiveEffects
            .Include(e => e.Inventory)
            .Where(e => e.PlayerUserId == playerUserId && e.IsActive)
            .ToListAsync();

        foreach (var effect in effects)
        {
            var expired = effect.ExpiresAt.HasValue && effect.ExpiresAt.Value <= now;
            var depleted = effect.RewardItemTypeId == RewardItemTypeIds.StreakProtection
                && effect.RemainingCharges is <= 0;

            if (!expired && !depleted)
                continue;

            effect.IsActive = false;
            if (effect.Inventory is not null)
                effect.Inventory.IsEquipped = false;
        }

        if (effects.Count > 0)
            await _context.SaveChangesAsync();
    }

    public async Task<decimal> GetActiveXpMultiplierAsync(int playerUserId)
    {
        await DeactivateExpiredEffectsAsync(playerUserId);

        var multipliers = await _context.PlayerActiveEffects
            .Where(e => e.PlayerUserId == playerUserId
                && e.IsActive
                && e.RewardItemTypeId == RewardItemTypeIds.XpBoost
                && e.EffectValue.HasValue)
            .Select(e => e.EffectValue!.Value)
            .ToListAsync();

        return multipliers.Count == 0 ? 1m : multipliers.Max();
    }

    public async Task<bool> TryConsumeStreakShieldAsync(int playerUserId, int missedDays)
    {
        if (missedDays <= 0)
            return false;

        await DeactivateExpiredEffectsAsync(playerUserId);

        var shield = await _context.PlayerActiveEffects
            .Include(e => e.Inventory)
            .Where(e => e.PlayerUserId == playerUserId
                && e.IsActive
                && e.RewardItemTypeId == RewardItemTypeIds.StreakProtection
                && e.RemainingCharges.HasValue
                && e.RemainingCharges.Value > 0)
            .OrderByDescending(e => e.RemainingCharges)
            .FirstOrDefaultAsync();

        if (shield is null || shield.RemainingCharges < missedDays)
            return false;

        shield.RemainingCharges -= missedDays;

        if (shield.RemainingCharges <= 0)
        {
            shield.IsActive = false;
            if (shield.Inventory is not null)
                shield.Inventory.IsEquipped = false;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public int CalculateXpPenalty(int currentExperiencePoints)
    {
        var percentPenalty = (int)Math.Round(currentExperiencePoints * 0.10m);
        return Math.Max(25, percentPenalty);
    }
}
