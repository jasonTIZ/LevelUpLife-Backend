namespace LevelUpLifeBackend.Services;

public interface IAvatarStorageService
{
    Task<string> SaveAvatarAsync(int playerUserId, Stream content, string contentType, string fileExtension);

    Task DeleteAvatarIfExistsAsync(string? avatarUrl);
}
