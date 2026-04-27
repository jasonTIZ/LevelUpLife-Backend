using LevelUpLifeBackend.Data;
using LevelUpLifeBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace LevelUpLifeBackend.Repositories;

public class PlayerRepository : IPlayerRepository
{
    private readonly AppDbContext _context;

    public PlayerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PlayerUser?> GetActiveByIdWithRelationsAsync(int playerUserId)
    {
        return await _context.PlayerUsers
            .Include(u => u.Person)
            .Include(u => u.Class)
            .FirstOrDefaultAsync(u => u.Id == playerUserId && u.IsActive);
    }

    public async Task<bool> UserNameExistsForAnotherUserAsync(string userName, int excludedPlayerUserId)
    {
        return await _context.PlayerUsers.AnyAsync(u =>
            u.Id != excludedPlayerUserId && u.UserName.ToLower() == userName.ToLower());
    }

    public async Task<UserPlayerClass?> GetClassByIdAsync(int classId)
    {
        return await _context.UserPlayerClasses.FirstOrDefaultAsync(c => c.Id == classId && c.IsActive);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
