using LevelUpLifeBackend.Data;
using LevelUpLifeBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace LevelUpLifeBackend.Repositories;

public class RepetitionCriteriaRepository : IRepetitionCriteriaRepository
{
    private readonly AppDbContext _context;

    public RepetitionCriteriaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<RepetitionCriteria> AddAsync(RepetitionCriteria criteria)
    {
        await _context.RepetitionCriteriaRecords.AddAsync(criteria);
        await _context.SaveChangesAsync();
        return criteria;
    }

    public async Task<RepetitionCriteria?> GetByTaskIdAsync(int taskId)
    {
        return await _context.RepetitionCriteriaRecords
            .FirstOrDefaultAsync(c => c.HabitTaskId == taskId);
    }

    public async Task<RepetitionCriteria> UpdateAsync(RepetitionCriteria criteria)
    {
        await _context.SaveChangesAsync();
        return criteria;
    }
}
