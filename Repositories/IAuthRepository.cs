using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.Repositories;

public interface IAuthRepository
{
    // Busca al usuario por UserName o por el Email de su Person.
    Task<PlayerUser?> GetByUserNameOrEmailAsync(string userNameOrEmail);

    Task<bool> EmailExistsAsync(string email);
    Task<bool> UserNameExistsAsync(string userName);
    Task<UserPlayerClass?> GetClassByIdAsync(int classId);
    Task RegisterAsync(Person person, PlayerUser playerUser);
}
