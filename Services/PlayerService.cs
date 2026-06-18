using System.Security.Cryptography;
using System.Text;
using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.DTOs.Responses;
using LevelUpLifeBackend.Models;
using LevelUpLifeBackend.Repositories;

namespace LevelUpLifeBackend.Services;

public class PlayerService : IPlayerService
{
    private readonly IPlayerRepository _playerRepository;
    private readonly ILevelProgressService _levelProgressService;

    public PlayerService(
        IPlayerRepository playerRepository,
        ILevelProgressService levelProgressService)
    {
        _playerRepository = playerRepository;
        _levelProgressService = levelProgressService;
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

        // Si la cuenta ya está inactiva, rechazamos la operación.
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
            TotalExperiencePoints = levelProgress.TotalExperiencePoints,
            ExperiencePointsInCurrentLevel = levelProgress.ExperiencePointsInCurrentLevel,
            ExperiencePointsRequiredForNextLevel = levelProgress.ExperiencePointsRequiredForNextLevel,
            LevelProgressPercent = levelProgress.LevelProgressPercent,
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

    private static PlayerProfileDto MapToProfileDto(PlayerUser player)
    {
        return new PlayerProfileDto
        {
            Id = player.Id,
            UserName = player.UserName,
            Level = player.Level,
            IsActive = player.IsActive,
            LastLogin = NormalizeLastLogin(player.LastLogin),
            ClassId = player.Class.Id,
            ClassName = player.Class.Name,
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
            player.Person.IsActive
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
