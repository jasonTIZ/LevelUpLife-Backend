using LevelUpLifeBackend.Data;
using LevelUpLifeBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace LevelUpLifeBackend.Repositories;

public class PlayerActiveEffectRepository : IPlayerActiveEffectRepository
{
    private readonly AppDbContext _context;

    public PlayerActiveEffectRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PlayerActiveEffect>> GetActiveByPlayerAsync(int playerUserId)
    {
        return await _context.PlayerActiveEffects
            .Include(e => e.RewardItem)
                .ThenInclude(r => r!.Type)
            .Include(e => e.Inventory)
            .Where(e => e.PlayerUserId == playerUserId && e.IsActive)
            .OrderByDescending(e => e.ActivatedAt)
            .ToListAsync();
    }

    public async Task<PlayerActiveEffect?> GetActiveByInventoryIdAsync(int inventoryId)
    {
        return await _context.PlayerActiveEffects
            .FirstOrDefaultAsync(e => e.InventoryId == inventoryId && e.IsActive);
    }

    public async Task<PlayerActiveEffect> AddAsync(PlayerActiveEffect effect)
    {
        await _context.PlayerActiveEffects.AddAsync(effect);
        await _context.SaveChangesAsync();
        await _context.Entry(effect).Reference(e => e.RewardItem).LoadAsync();
        if (effect.RewardItem is not null)
            await _context.Entry(effect.RewardItem).Reference(r => r.Type).LoadAsync();
        return effect;
    }

    public async Task UpdateAsync(PlayerActiveEffect effect)
    {
        await _context.SaveChangesAsync();
    }
}
