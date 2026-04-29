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
}