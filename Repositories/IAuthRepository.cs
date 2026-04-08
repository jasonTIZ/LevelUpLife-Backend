using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.Repositories;

public interface IAuthRepository
{
    // Busca al usuario por UserName o por el Email de su Person.
    Task<PlayerUser?> GetByUserNameOrEmailAsync(string userNameOrEmail);
}
