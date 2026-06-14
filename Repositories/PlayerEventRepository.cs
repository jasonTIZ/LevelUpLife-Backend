using LevelUpLifeBackend.Data;
using LevelUpLifeBackend.Models;

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
}
