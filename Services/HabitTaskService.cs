using LevelUpLifeBackend.DTOs.Responses;
using LevelUpLifeBackend.Infrastructure.Errors;
using LevelUpLifeBackend.Repositories;

namespace LevelUpLifeBackend.Services;

public class HabitTaskService : IHabitTaskService
{
    private readonly IHabitTaskRepository _habitTaskRepository;

    public HabitTaskService(IHabitTaskRepository habitTaskRepository)
    {
        _habitTaskRepository = habitTaskRepository;
    }

    public async Task<IEnumerable<EvidenceStorageResponseDto>> GetEvidencesByTaskIdAsync(int taskId)
    {
        try
        {
            if (!await _habitTaskRepository.ExistsAsync(taskId))
            {
                throw new NotFoundError(new ErrorResponse
                {
                    Code = 404,
                    Message = $"Task with ID {taskId} not found.",
                    Details = "The requested habit task does not exist in the database."
                });
            }

            var evidences = await _habitTaskRepository.GetEvidencesByTaskIdAsync(taskId);

            return evidences.Select(e => new EvidenceStorageResponseDto
            {
                Id = e.Id,
                HabitTaskId = e.HabitTaskId,
                EvidencePathUrl = e.EvidencePathUrl,
                HealthDataJson = e.HealthDataJson,
                UploadedAt = e.UploadedAt
            });
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
                Message = "An unexpected error occurred while fetching evidence records.",
                Details = ex.Message
            });
        }
    }

    public async Task<EvidenceStorageResponseDto> GetEvidenceByIdAsync(int taskId, int id)
    {
        try
        {
            if (!await _habitTaskRepository.ExistsAsync(taskId))
            {
                throw new NotFoundError(new ErrorResponse
                {
                    Code = 404,
                    Message = $"Task with ID {taskId} not found.",
                    Details = "The requested habit task does not exist in the database."
                });
            }

            var evidence = await _habitTaskRepository.GetEvidenceByIdAsync(taskId, id);

            if (evidence == null)
            {
                throw new NotFoundError(new ErrorResponse
                {
                    Code = 404,
                    Message = $"Evidence with ID {id} not found for Task {taskId}.",
                    Details = "The requested evidence record does not exist or does not belong to the specified task."
                });
            }

            return new EvidenceStorageResponseDto
            {
                Id = evidence.Id,
                HabitTaskId = evidence.HabitTaskId,
                EvidencePathUrl = evidence.EvidencePathUrl,
                HealthDataJson = evidence.HealthDataJson,
                UploadedAt = evidence.UploadedAt
            };
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
                Message = "An unexpected error occurred while fetching the evidence record.",
                Details = ex.Message
            });
        }
    }
}
