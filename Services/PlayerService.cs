using System.Security.Cryptography;
using System.Text;
using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.DTOs.Responses;
using LevelUpLifeBackend.Infrastructure;
using LevelUpLifeBackend.Infrastructure.Configuration;
using LevelUpLifeBackend.Models;
using LevelUpLifeBackend.Repositories;
using Microsoft.Extensions.Options;

namespace LevelUpLifeBackend.Services;

public class PlayerService : IPlayerService
{
    private readonly IPlayerRepository _playerRepository;
    private readonly ILevelProgressService _levelProgressService;
    private readonly IAvatarStorageService _avatarStorageService;
    private readonly PlayerProfileOptions _profileOptions;

    public PlayerService(
        IPlayerRepository playerRepository,
        ILevelProgressService levelProgressService,
        IAvatarStorageService avatarStorageService,
        IOptions<PlayerProfileOptions> profileOptions)
    {
        _playerRepository = playerRepository;
        _levelProgressService = levelProgressService;
        _avatarStorageService = avatarStorageService;
        _profileOptions = profileOptions.Value;
    }

    public async Task<GetPlayerProfileServiceResult> GetProfileAsync(int playerUserId)
    {
        var player = await _playerRepository.GetActiveByIdWithRelationsAsync(playerUserId);
        if (player is null)
        {
            return new GetPlayerProfileServiceResult { Status = GetPlayerProfileStatus.NotFound };
        }

        return new GetPlayerProfileServiceResult
        {
            Status = GetPlayerProfileStatus.Success,
            Response = MapToGetProfileResponse(player),
            ETag = GenerateETag(player)
        };
    }

    public async Task<UpdatePlayerProfileServiceResult> UpdateProfileAsync(
        int playerUserId,
        string ifMatchHeader,
        UpdatePlayerProfileRequestDto request
    )
    {
        var player = await _playerRepository.GetActiveByIdWithRelationsAsync(playerUserId);
        if (player is null)
        {
            return new UpdatePlayerProfileServiceResult { Status = UpdatePlayerProfileStatus.NotFound };
        }

        var currentETag = GenerateETag(player);
        if (!ETagMatches(ifMatchHeader, currentETag))
        {
            return new UpdatePlayerProfileServiceResult
            {
                Status = UpdatePlayerProfileStatus.ETagMismatch,
                ETag = currentETag
            };
        }

        if (request.PlayerData?.UserName is not null)
        {
            var newUserName = request.PlayerData.UserName.Trim();
            if (newUserName.Length == 0)
            {
                return new UpdatePlayerProfileServiceResult
                {
                    Status = UpdatePlayerProfileStatus.InvalidData,
                    Details = "El username no puede quedar vacío."
                };
            }

            var isTaken = await _playerRepository.UserNameExistsForAnotherUserAsync(newUserName, player.Id);
            if (isTaken)
            {
                return new UpdatePlayerProfileServiceResult
                {
                    Status = UpdatePlayerProfileStatus.UsernameTaken
                };
            }

            player.UserName = newUserName;
        }

        if (request.PlayerData?.PreferredClassId is int preferredClassId)
        {
            var selectedClass = await _playerRepository.GetClassByIdAsync(preferredClassId);
            if (selectedClass is null)
            {
                return new UpdatePlayerProfileServiceResult
                {
                    Status = UpdatePlayerProfileStatus.InvalidData,
                    Details = "La clase seleccionada no existe o está inactiva."
                };
            }

            player.Class = selectedClass;
        }

        if (request.PlayerData?.Bio is not null)
        {
            var bio = request.PlayerData.Bio.Trim();
            if (bio.Length > _profileOptions.MaxBioLength)
            {
                return new UpdatePlayerProfileServiceResult
                {
                    Status = UpdatePlayerProfileStatus.InvalidData,
                    Details = $"La bio no puede superar {_profileOptions.MaxBioLength} caracteres."
                };
            }

            player.Bio = bio.Length == 0 ? null : bio;
        }

        if (request.PersonData is not null)
        {
            if (request.PersonData.Name is not null)
                player.Person.Name = request.PersonData.Name.Trim();
            if (request.PersonData.LastName is not null)
                player.Person.LastName = request.PersonData.LastName.Trim();
            if (request.PersonData.Email is not null)
                player.Person.Email = request.PersonData.Email.Trim();
            if (request.PersonData.Birthdate is DateOnly birthDate)
                player.Person.BirthDate = birthDate;
        }

        await _playerRepository.SaveChangesAsync();

        var response = new UpdatePlayerProfileResponseDto
        {
            Success = true,
            Message = "Perfil actualizado correctamente",
            Player = MapToProfileDto(player)
        };

        return new UpdatePlayerProfileServiceResult
        {
            Status = UpdatePlayerProfileStatus.Success,
            Response = response,
            ETag = GenerateETag(player)
        };
    }

    public async Task<UploadPlayerAvatarServiceResult> UploadAvatarAsync(
        int playerUserId,
        string ifMatchHeader,
        Stream avatarContent,
        string contentType,
        long contentLength)
    {
        var player = await _playerRepository.GetActiveByIdWithRelationsAsync(playerUserId);
        if (player is null)
        {
            return new UploadPlayerAvatarServiceResult { Status = UploadPlayerAvatarStatus.NotFound };
        }

        var currentETag = GenerateETag(player);
        if (!ETagMatches(ifMatchHeader, currentETag))
        {
            return new UploadPlayerAvatarServiceResult
            {
                Status = UploadPlayerAvatarStatus.ETagMismatch,
                ETag = currentETag
            };
        }

        if (contentLength <= 0)
        {
            return InvalidAvatarUpload("Debe enviar un archivo de imagen.");
        }

        if (contentLength > _profileOptions.MaxAvatarBytes)
        {
            return InvalidAvatarUpload(
                $"La imagen supera el tamaño máximo permitido ({_profileOptions.MaxAvatarBytes} bytes).");
        }

        if (!AvatarContentTypeValidator.IsAllowedContentType(
                contentType,
                _profileOptions.AllowedAvatarContentTypes))
        {
            return InvalidAvatarUpload(
                "Tipo de imagen no permitido. Use JPEG, PNG o WEBP.");
        }

        if (!AvatarContentTypeValidator.HasValidSignature(avatarContent, contentType))
        {
            return InvalidAvatarUpload("El contenido del archivo no coincide con el tipo declarado.");
        }

        var extension = AvatarContentTypeValidator.ResolveExtension(contentType);
        if (string.IsNullOrEmpty(extension))
        {
            return InvalidAvatarUpload("Tipo de imagen no permitido. Use JPEG, PNG o WEBP.");
        }

        await _avatarStorageService.DeleteAvatarIfExistsAsync(player.AvatarUrl);
        var avatarUrl = await _avatarStorageService.SaveAvatarAsync(
            playerUserId,
            avatarContent,
            contentType,
            extension);

        player.AvatarUrl = avatarUrl;
        await _playerRepository.SaveChangesAsync();

        return new UploadPlayerAvatarServiceResult
        {
            Status = UploadPlayerAvatarStatus.Success,
            Response = new UpdatePlayerProfileResponseDto
            {
                Success = true,
                Message = "Avatar actualizado correctamente",
                Player = MapToProfileDto(player)
            },
            ETag = GenerateETag(player)
        };
    }

    public async Task<DeletePlayerAccountServiceResult> DeleteAccountAsync(int playerUserId, string? reason)
    {
        var player = await _playerRepository.GetByIdWithRelationsAsync(playerUserId);
        if (player is null)
        {
            return new DeletePlayerAccountServiceResult
            {
                Status = DeletePlayerAccountStatus.NotFound
            };
        }

        if (!player.IsActive)
        {
            return new DeletePlayerAccountServiceResult
            {
                Status = DeletePlayerAccountStatus.Forbidden
            };
        }

        player.IsActive = false;
        if (player.Person is not null)
        {
            player.Person.IsActive = false;
        }

        await _playerRepository.SaveChangesAsync();

        return new DeletePlayerAccountServiceResult
        {
            Status = DeletePlayerAccountStatus.Success,
            Response = new DeletePlayerAccountResponseDto
            {
                Success = true,
                Message = "Cuenta desactivada correctamente",
                DeactivatedAt = DateTime.UtcNow
            }
        };
    }

    private UploadPlayerAvatarServiceResult InvalidAvatarUpload(string details) =>
        new()
        {
            Status = UploadPlayerAvatarStatus.InvalidData,
            Details = details,
        };

    private static DateTime? NormalizeLastLogin(DateTime? lastLogin) =>
        lastLogin is { Year: > 1 } ? lastLogin : null;

    private GetPlayerProfileResponseDto MapToGetProfileResponse(PlayerUser player)
    {
        var levelProgress = _levelProgressService.GetLevelProgress(player.ExperiencePoints);

        return new GetPlayerProfileResponseDto
        {
            PlayerUserId = player.Id.ToString(),
            PlayerUserUserName = player.UserName,
            PlayerUserLevel = levelProgress.CurrentLevel,
            StatusIsActive = player.IsActive,
            PlayerUserLastLogin = NormalizeLastLogin(player.LastLogin),
            PlayerUserCreationDate = player.CreationDate,
            Bio = player.Bio,
            AvatarUrl = player.AvatarUrl,
            Gold = player.Gold,
            TotalExperiencePoints = levelProgress.TotalExperiencePoints,
            ExperiencePointsInCurrentLevel = levelProgress.ExperiencePointsInCurrentLevel,
            ExperiencePointsRequiredForNextLevel = levelProgress.ExperiencePointsRequiredForNextLevel,
            LevelProgressPercent = levelProgress.LevelProgressPercent,
            DaysStreak = player.DaysStreak,
            LevelingConfig = _levelProgressService.GetLevelingConfig(),
            PersonData = new GetPlayerProfilePersonDataDto
            {
                Name = player.Person.Name,
                LastName = player.Person.LastName,
                Email = player.Person.Email,
                Birthdate = player.Person.BirthDate
            }
        };
    }

    private PlayerProfileDto MapToProfileDto(PlayerUser player)
    {
        var levelProgress = _levelProgressService.GetLevelProgress(player.ExperiencePoints);

        return new PlayerProfileDto
        {
            Id = player.Id,
            UserName = player.UserName,
            Level = levelProgress.CurrentLevel,
            IsActive = player.IsActive,
            LastLogin = NormalizeLastLogin(player.LastLogin),
            ClassId = player.Class.Id,
            ClassName = player.Class.Name,
            Bio = player.Bio,
            AvatarUrl = player.AvatarUrl,
            CreationDate = player.CreationDate,
            Person = new PersonProfileDto
            {
                Id = player.Person.Id,
                Name = player.Person.Name,
                LastName = player.Person.LastName,
                Email = player.Person.Email,
                Birthdate = player.Person.BirthDate,
                IsActive = player.Person.IsActive
            }
        };
    }

    private static string GenerateETag(PlayerUser player)
    {
        var canonical = string.Join(
            "|",
            player.Id,
            player.UserName,
            player.Level,
            player.IsActive,
            NormalizeLastLogin(player.LastLogin)?.ToUniversalTime().ToString("O") ?? "",
            player.Class.Id,
            player.Class.Name,
            player.Person.Id,
            player.Person.Name,
            player.Person.LastName,
            player.Person.Email,
            player.Person.BirthDate?.ToString("yyyy-MM-dd") ?? "",
            player.Person.IsActive,
            player.Bio ?? "",
            player.AvatarUrl ?? "",
            player.CreationDate.ToUniversalTime().ToString("O")
        );

        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(canonical));
        var hash = Convert.ToHexString(hashBytes).ToLowerInvariant();
        return $"\"{hash}\"";
    }

    private static bool ETagMatches(string ifMatchHeader, string currentETag)
    {
        if (ifMatchHeader == "*")
            return true;

        var normalizedIfMatch = NormalizeETag(ifMatchHeader);
        var normalizedCurrent = NormalizeETag(currentETag);
        return normalizedIfMatch == normalizedCurrent;
    }

    private static string NormalizeETag(string etag)
    {
        var value = etag.Trim();
        if (value.StartsWith("W/"))
            value = value[2..];
        return value.Trim().Trim('"');
    }
}
