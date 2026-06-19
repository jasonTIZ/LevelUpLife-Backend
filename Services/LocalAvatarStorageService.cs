using LevelUpLifeBackend.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace LevelUpLifeBackend.Services;

public class LocalAvatarStorageService : IAvatarStorageService
{
    private readonly PlayerProfileOptions _options;
    private readonly IWebHostEnvironment _environment;

    public LocalAvatarStorageService(
        IOptions<PlayerProfileOptions> options,
        IWebHostEnvironment environment)
    {
        _options = options.Value;
        _environment = environment;
    }

    public async Task<string> SaveAvatarAsync(
        int playerUserId,
        Stream content,
        string contentType,
        string fileExtension)
    {
        var playerDirectory = Path.Combine(
            _environment.ContentRootPath,
            _options.AvatarStoragePath,
            playerUserId.ToString());

        Directory.CreateDirectory(playerDirectory);

        foreach (var existingFile in Directory.EnumerateFiles(playerDirectory))
        {
            File.Delete(existingFile);
        }

        var fileName = $"{Guid.NewGuid():N}{fileExtension}";
        var absolutePath = Path.Combine(playerDirectory, fileName);

        await using var fileStream = File.Create(absolutePath);
        await content.CopyToAsync(fileStream);

        var publicUrl = $"{_options.AvatarPublicBasePath.TrimEnd('/')}/{playerUserId}/{fileName}";
        return publicUrl;
    }

    public Task DeleteAvatarIfExistsAsync(string? avatarUrl)
    {
        if (string.IsNullOrWhiteSpace(avatarUrl))
        {
            return Task.CompletedTask;
        }

        var relativePath = avatarUrl
            .Replace(_options.AvatarPublicBasePath.TrimEnd('/'), string.Empty, StringComparison.OrdinalIgnoreCase)
            .TrimStart('/');

        var absolutePath = Path.Combine(
            _environment.ContentRootPath,
            _options.AvatarStoragePath,
            relativePath.Replace('/', Path.DirectorySeparatorChar));

        if (File.Exists(absolutePath))
        {
            File.Delete(absolutePath);
        }

        return Task.CompletedTask;
    }
}
