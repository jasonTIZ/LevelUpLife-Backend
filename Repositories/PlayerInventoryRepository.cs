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

    public async Task<IEnumerable<PlayerInventory>> GetByPlayerIdAsync(int playerUserId)
    {
        return await _context.PlayerInventories
            .AsNoTracking()
            .Include(i => i.RewardItem)
                .ThenInclude(r => r!.Type)
            .Where(i => i.PlayerUserId == playerUserId)
            .OrderByDescending(i => i.AcquiredAt)
            .ToListAsync();
    }
}
