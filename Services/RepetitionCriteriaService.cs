using LevelUpLifeBackend.Infrastructure.Errors;
using LevelUpLifeBackend.Repositories;

namespace LevelUpLifeBackend.Services;

public class RepetitionCriteriaService : IRepetitionCriteriaService
{
    private readonly IRepetitionCriteriaRepository _repetitionCriteriaRepository;

    public RepetitionCriteriaService(IRepetitionCriteriaRepository repetitionCriteriaRepository)
    {
        _repetitionCriteriaRepository = repetitionCriteriaRepository;
    }

    public async Task DeactivateAsync(int taskId, int id)
    {
        try
        {
            var criteria = await _repetitionCriteriaRepository.GetByTaskIdAsync(taskId);

            if (criteria == null || criteria.Id != id)
            {
                throw new NotFoundError(new ErrorResponse
                {
                    Code = 404,
                    Message = $"Repetition criteria with ID {id} not found for task {taskId}.",
                    Details = "The requested repetition criteria does not exist or does not belong to the specified task."
                });
            }

            criteria.StatusRepetitionsCriteriaIsActive = false;
            await _repetitionCriteriaRepository.UpdateAsync(criteria);
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
                Message = "An unexpected error occurred while deactivating repetition criteria.",
                Details = ex.Message
            });
        }
    }
}
