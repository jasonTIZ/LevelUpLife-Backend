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

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Persons
            .AnyAsync(p => p.Email.ToLower() == email.ToLower());
    }

    public async Task<bool> UserNameExistsAsync(string userName)
    {
        return await _context.PlayerUsers
            .AnyAsync(u => u.UserName.ToLower() == userName.ToLower());
    }

    public async Task<UserPlayerClass?> GetClassByIdAsync(int classId)
    {
        return await _context.UserPlayerClasses
            .FirstOrDefaultAsync(c => c.Id == classId && c.IsActive);
    }

    public async Task RegisterAsync(Person person, PlayerUser playerUser)
    {
        _context.Persons.Add(person);
        await _context.SaveChangesAsync();

        playerUser.Person = person;
        _context.PlayerUsers.Add(playerUser);
        await _context.SaveChangesAsync();
    }
}
