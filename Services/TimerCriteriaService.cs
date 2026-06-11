using LevelUpLifeBackend.Infrastructure.Errors;
using LevelUpLifeBackend.Repositories;

namespace LevelUpLifeBackend.Services;

public class TimerCriteriaService : ITimerCriteriaService
{
    private readonly ITimerCriteriaRepository _timerCriteriaRepository;

    public TimerCriteriaService(ITimerCriteriaRepository timerCriteriaRepository)
    {
        _timerCriteriaRepository = timerCriteriaRepository;
    }

    public async Task DeactivateAsync(int taskId, int id)
    {
        try
        {
            var criteria = await _timerCriteriaRepository.GetByTaskIdAsync(taskId);

            if (criteria == null || criteria.Id != id)
            {
                throw new NotFoundError(new ErrorResponse
                {
                    Code = 404,
                    Message = $"Timer criteria with ID {id} not found for task {taskId}.",
                    Details = "The requested timer criteria does not exist or does not belong to the specified task."
                });
            }

            criteria.StatusTimerCriteriaIsActive = false;
            await _timerCriteriaRepository.UpdateAsync(criteria);
        }
        catch (NotFoundError)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ServerError(500, new ErrorResponse
            {
                Code = 500,
                Message = "An unexpected error occurred while deactivating timer criteria.",
                Details = ex.Message
            });
        }
    }
}
