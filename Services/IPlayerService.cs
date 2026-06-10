using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.DTOs.Responses;

namespace LevelUpLifeBackend.Services;

public interface IPlayerService
{
    Task<GetPlayerProfileServiceResult> GetProfileAsync(int playerUserId);

    Task<UpdatePlayerProfileServiceResult> UpdateProfileAsync(
        int playerUserId,
        string ifMatchHeader,
        UpdatePlayerProfileRequestDto request
    );

    Task<DeletePlayerAccountServiceResult> DeleteAccountAsync(int playerUserId, string? reason);
}

public enum GetPlayerProfileStatus
{
    Success,
    NotFound
}

public class GetPlayerProfileServiceResult
{
    public GetPlayerProfileStatus Status { get; set; }
    public GetPlayerProfileResponseDto? Response { get; set; }
    public string? ETag { get; set; }
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

public enum DeletePlayerAccountStatus
{
    Success,
    NotFound,
    Forbidden
}

public class DeletePlayerAccountServiceResult
{
    public DeletePlayerAccountStatus Status { get; set; }
    public DeletePlayerAccountResponseDto? Response { get; set; }
}
