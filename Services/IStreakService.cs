using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.DTOs.Responses;

namespace LevelUpLifeBackend.Services;

public interface IStreakService
{
    Task<ActivateStreakProtectionResponseDto> ActivateProtectionAsync(
        int userId,
        ActivateStreakProtectionRequestDto request
    );
}
