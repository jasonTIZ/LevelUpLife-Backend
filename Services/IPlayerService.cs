using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.DTOs.Responses;

namespace LevelUpLifeBackend.Services;

public interface IPlayerService
{
    Task<UpdatePlayerProfileServiceResult> UpdateProfileAsync(
        int playerUserId,
        string ifMatchHeader,
        UpdatePlayerProfileRequestDto request
    );
}

public enum UpdatePlayerProfileStatus
{
    Success,
    NotFound,
    UsernameTaken,
    ETagMismatch,
    InvalidData
}

public class UpdatePlayerProfileServiceResult
{
    public UpdatePlayerProfileStatus Status { get; set; }
    public UpdatePlayerProfileResponseDto? Response { get; set; }
    public string? ETag { get; set; }
    public string? Details { get; set; }
}
