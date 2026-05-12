using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.DTOs.Responses;

namespace LevelUpLifeBackend.Services;

public interface IRewardItemService
{
    Task<IEnumerable<RewardItemResponseDto>> GetFilteredAsync(RewardItemFilterRequestDto? filter);
}
