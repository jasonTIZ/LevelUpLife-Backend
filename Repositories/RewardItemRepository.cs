using LevelUpLifeBackend.Data;
using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace LevelUpLifeBackend.Repositories;

public class RewardItemRepository : IRewardItemRepository
{
    private readonly AppDbContext _context;

    public RewardItemRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RewardItem>> GetFilteredAsync(RewardItemFilterRequestDto? filter)
    {
        var query = _context.RewardItems
            .AsNoTracking()
            .Include(ri => ri.Type)
            .Where(ri => ri.IsActive);

        if (filter != null)
        {
            if (filter.TypeId.HasValue)
                query = query.Where(ri => ri.TypeId == filter.TypeId.Value);

            if (!string.IsNullOrWhiteSpace(filter.Name))
                query = query.Where(ri => ri.Name.Contains(filter.Name));

            if (!string.IsNullOrWhiteSpace(filter.Description))
                query = query.Where(ri => ri.Description != null && ri.Description.Contains(filter.Description));

            if (filter.CostGold.HasValue)
                query = query.Where(ri => ri.CostGold == filter.CostGold.Value);

            if (filter.EffectValue.HasValue)
                query = query.Where(ri => ri.EffectValue == (decimal)filter.EffectValue.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<RewardItem?> GetActiveByIdAsync(int rewardItemId)
    {
        return await _context.RewardItems
            .Include(ri => ri.Type)
            .FirstOrDefaultAsync(ri => ri.Id == rewardItemId && ri.IsActive);
    }
}
