using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.DTOs.Responses;
using LevelUpLifeBackend.Infrastructure.Errors;
using LevelUpLifeBackend.Mappers;
using LevelUpLifeBackend.Repositories;

namespace LevelUpLifeBackend.Services;

public class TimerCriteriaService : ITimerCriteriaService
{
    private readonly ITimerCriteriaRepository _timerCriteriaRepository;
    private readonly IHabitTaskRepository _habitTaskRepository;

    public TimerCriteriaService(
        ITimerCriteriaRepository timerCriteriaRepository,
        IHabitTaskRepository habitTaskRepository)
    {
        _timerCriteriaRepository = timerCriteriaRepository;
        _habitTaskRepository = habitTaskRepository;
    }

    public async Task<TimerCriteriaResponseDto> CreateAsync(int taskId, CreateTimerCriteriaRequestDto request)
    {
        try
        {
            var taskExists = await _habitTaskRepository.ExistsAsync(taskId);
            if (!taskExists)
            {
                throw new NotFoundError(new ErrorResponse
                {
                    Code = 404,
                    Message = $"Habit task with ID {taskId} not found.",
                    Details = "The specified task does not exist."
                });
            }

            var existing = await _timerCriteriaRepository.GetByTaskIdAsync(taskId);
            if (existing is not null)
            {
                throw new ConflictError(new ErrorResponse
                {
                    Code = 409,
                    Message = "CRITERIA_ALREADY_EXISTS",
                    Details = $"A timer criteria already exists for task {taskId}."
                });
            }

            var entity = TimerCriteriaMapper.ToEntity(taskId, request);
            var created = await _timerCriteriaRepository.AddAsync(entity);
            return TimerCriteriaMapper.ToResponse(created);
        }
        catch (NotFoundError)
        {
            throw;
        }
        catch (ConflictError)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ServerError(500, new ErrorResponse
            {
                Code = 500,
                Message = "An unexpected error occurred while creating the timer criteria.",
                Details = ex.Message
            });
        }
    }
}
