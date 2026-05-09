using LevelUpLifeBackend.DTOs.Responses;
using LevelUpLifeBackend.Infrastructure.Errors;
using LevelUpLifeBackend.Repositories;

namespace LevelUpLifeBackend.Services;

public class HabitDisciplineService : IHabitDisciplineService
{
    private readonly IHabitDisciplineRepository _habitDisciplineRepository;

    public HabitDisciplineService(IHabitDisciplineRepository habitDisciplineRepository)
    {
        _habitDisciplineRepository = habitDisciplineRepository;
    }

    public async Task<HabitDisciplineDetailResponseDto> GetDisciplineByIdAsync(int id)
    {
        try
        {
            var discipline = await _habitDisciplineRepository.GetByIdAsync(id);

            if (discipline is null)
            {
                throw new NotFoundError(
                    new ErrorResponse
                    {
                        Code = 404,
                        Message = $"Discipline with ID {id} not found.",
                        Details = "The requested habit discipline does not exist in the database.",
                    }
                );
            }

            return new HabitDisciplineDetailResponseDto
            {
                IdHabitDiscipline = discipline.Id,
                IdHabitCategory = discipline.Category.Id,
                DscHabitDisciplineName = discipline.Name,
                DscHabitDisciplineDescription = discipline.Description,
                StatusHabitDisciplineIsActive = discipline.IsActive,
            };
        }
        catch (NotFoundError)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ServerError(
                500,
                new ErrorResponse
                {
                    Code = 500,
                    Message = "An unexpected error occurred while fetching the habit discipline.",
                    Details = ex.Message,
                }
            );
        }
    }
}
