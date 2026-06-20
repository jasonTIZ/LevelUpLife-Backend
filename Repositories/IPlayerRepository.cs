using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.Repositories;

public interface IPlayerRepository
{
    Task<PlayerUser?> GetActiveByIdAsync(int playerUserId);
    Task<PlayerUser?> GetActiveByIdWithRelationsAsync(int playerUserId);
    Task<PlayerUser?> GetByIdWithRelationsAsync(int playerUserId);
    Task<bool> UserNameExistsForAnotherUserAsync(string userName, int excludedPlayerUserId);
    Task<UserPlayerClass?> GetClassByIdAsync(int classId);
    Task SaveChangesAsync();
}
