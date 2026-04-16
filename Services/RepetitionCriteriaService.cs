using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.DTOs.Responses;
using LevelUpLifeBackend.Mappers;
using LevelUpLifeBackend.Repositories;
using LevelUpLifeBackend.Exceptions;

namespace LevelUpLifeBackend.Services;

public class RepetitionCriteriaService : IRepetitionCriteriaService
{
    private readonly IRepetitionCriteriaRepository _repository;

    public RepetitionCriteriaService(IRepetitionCriteriaRepository repository)
    {
        _repository = repository;
    }

    public async Task<RepetitionCriteriaResponseDto?> CreateAsync(int taskId, CreateRepetitionCriteriaRequestDto request)
    {
        var taskExists = await _repository.HabitTaskExistsAsync(taskId);
        if (!taskExists)
        {
            return null;
        }

        var criteriaExists = await _repository.CriteriaExistsAsync(taskId);
        if (criteriaExists)
        {
            throw new RepetitionCriteriaAlreadyExistsException();
        }

        var createdCriteria = await _repository.AddAsync(RepetitionCriteriaMapper.ToEntity(taskId, request));
        return RepetitionCriteriaMapper.ToResponse(createdCriteria);
    }
}