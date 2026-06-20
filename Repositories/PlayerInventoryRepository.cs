using LevelUpLifeBackend.Data;
using LevelUpLifeBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace LevelUpLifeBackend.Repositories;

public class PlayerInventoryRepository : IPlayerInventoryRepository
{
    private readonly AppDbContext _context;

    public PlayerInventoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PlayerInventory?> GetByIdForPlayerAsync(int inventoryId, int playerUserId)
    {
        return await _context.PlayerInventories
            .Include(i => i.RewardItem)
                .ThenInclude(r => r!.Type)
            .FirstOrDefaultAsync(i => i.Id == inventoryId && i.PlayerUserId == playerUserId);
    }

    public async Task<PlayerInventory?> GetByPlayerAndItemAsync(int playerUserId, int rewardItemId)
    {
        return await _context.PlayerInventories
            .Include(i => i.RewardItem)
                .ThenInclude(r => r!.Type)
            .FirstOrDefaultAsync(i => i.PlayerUserId == playerUserId && i.RewardItemId == rewardItemId);
    }

    public async Task<PlayerInventory> AddAsync(PlayerInventory entry)
    {
        _context.PlayerInventories.Add(entry);
        await _context.SaveChangesAsync();
        await _context.Entry(entry).Reference(i => i.RewardItem).LoadAsync();
        await _context.Entry(entry.RewardItem!).Reference(r => r.Type).LoadAsync();
        return entry;
    }

    public async Task<PlayerInventory> UpdateAsync(PlayerInventory entry)
    {
        await _context.SaveChangesAsync();
        return entry;
    }

    public async Task<IEnumerable<PlayerInventory>> GetByPlayerAsync(int playerUserId)
    {
        return await _context.PlayerInventories
            .AsNoTracking()
            .Include(i => i.RewardItem)
                .ThenInclude(r => r!.Type)
            .Where(i => i.PlayerUserId == playerUserId)
            .OrderBy(i => i.AcquiredAt)
            .ToListAsync();
    }
}
