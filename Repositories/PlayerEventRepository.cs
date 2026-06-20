using LevelUpLifeBackend.Data;
using LevelUpLifeBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace LevelUpLifeBackend.Repositories;

public class PlayerEventRepository : IPlayerEventRepository
{
    private readonly AppDbContext _context;

    public PlayerEventRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(PlayerEvent playerEvent)
    {
        await _context.PlayerEvents.AddAsync(playerEvent);
        await _context.SaveChangesAsync();
    }

    public async Task<PlayerEvent?> GetLastByTypeAsync(int playerUserId, PlayerEventType eventType)
    {
        return await _context.PlayerEvents
            .Where(e => e.PlayerUserId == playerUserId && e.EventType == eventType)
            .OrderByDescending(e => e.CreatedAt)
            .FirstOrDefaultAsync();
    }
}
