using LevelUpLifeBackend.Data;
using LevelUpLifeBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace LevelUpLifeBackend.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly AppDbContext _context;

    public AuthRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PlayerUser?> GetByUserNameOrEmailAsync(string userNameOrEmail)
    {
        // Get PlayerUser for UserName or for Email (Person).
        return await _context.PlayerUsers
            .Include(u => u.Person)
            .Include(u => u.Class)
            .FirstOrDefaultAsync(u =>
                u.UserName.ToLower() == userNameOrEmail.ToLower() ||
                u.Person.Email.ToLower() == userNameOrEmail.ToLower());
    }
}
