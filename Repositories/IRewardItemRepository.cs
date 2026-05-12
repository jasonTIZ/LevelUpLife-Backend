using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.Repositories;

public interface IRewardItemRepository
{
    Task<IEnumerable<RewardItem>> GetFilteredAsync(RewardItemFilterRequestDto? filter);
}
