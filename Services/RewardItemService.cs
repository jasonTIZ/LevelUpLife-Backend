using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.DTOs.Responses;
using LevelUpLifeBackend.Infrastructure.Errors;
using LevelUpLifeBackend.Repositories;

namespace LevelUpLifeBackend.Services;

public class RewardItemService : IRewardItemService
{
    private readonly IRewardItemRepository _rewardItemRepository;

    public RewardItemService(IRewardItemRepository rewardItemRepository)
    {
        _rewardItemRepository = rewardItemRepository;
    }

    public async Task<IEnumerable<RewardItemResponseDto>> GetFilteredAsync(RewardItemFilterRequestDto? filter)
    {
        try
        {
            var items = await _rewardItemRepository.GetFilteredAsync(filter);

            return items.Select(ri => new RewardItemResponseDto
            {
                Id = ri.Id,
                TypeId = ri.TypeId,
                TypeName = ri.Type?.Name ?? string.Empty,
                Name = ri.Name,
                Description = ri.Description,
                CostGold = ri.CostGold,
                EffectValue = ri.EffectValue,
                DurationDays = ri.DurationDays,
                IsActive = ri.IsActive,
            });
        }
        catch (Exception ex)
        {
            throw new ServerError(500, new ErrorResponse
            {
                Code = 500,
                Message = "An unexpected error occurred while fetching reward items.",
                Details = ex.Message
            });
        }
    }
}
