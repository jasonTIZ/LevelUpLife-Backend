using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.DTOs.Responses;
using LevelUpLifeBackend.Infrastructure.Errors;
using LevelUpLifeBackend.Models;
using LevelUpLifeBackend.Repositories;

namespace LevelUpLifeBackend.Services;

public class HabitDisciplineService : IHabitDisciplineService
{
    private readonly IHabitDisciplineRepository _habitDisciplineRepository;
    private readonly IHabitCategoryRepository _habitCategoryRepository;

    public HabitDisciplineService(
        IHabitDisciplineRepository habitDisciplineRepository,
        IHabitCategoryRepository habitCategoryRepository)
    {
        _habitDisciplineRepository = habitDisciplineRepository;
        _habitCategoryRepository = habitCategoryRepository;
    }

    public async Task<IEnumerable<HabitDisciplineDetailResponseDto>> GetAllDisciplinesAsync()
    {
        try
        {
            var disciplines = await _habitDisciplineRepository.GetAllAsync();
            return disciplines.Select(d => MapToDto(d));
        }
        catch (Exception ex)
        {
            throw new ServerError(
                500,
                new ErrorResponse
                {
                    Code = 500,
                    Message = "An unexpected error occurred while fetching habit disciplines.",
                    Details = ex.Message,
                }
            );
        }
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

            return MapToDto(discipline);
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

    public async Task<HabitDisciplineDetailResponseDto> CreateDisciplineAsync(
        CreateHabitDisciplineRequestDto request)
    {
        try
        {
            var category = await _habitCategoryRepository.GetByIdAsync(request.IdHabitCategory);
            if (category is null)
            {
                throw new AppError(
                    StatusCodes.Status400BadRequest,
                    new ErrorResponse
                    {
                        Code = 400,
                        Message = "Validation failed.",
                        Details = "The specified habit category does not exist.",
                    }
                );
            }

            var discipline = new HabitDiscipline
            {
                Category = new HabitCategory { Id = request.IdHabitCategory },
                Name = request.DscHabitDisciplineName.Trim(),
                Description = request.DscHabitDisciplineDescription?.Trim() ?? string.Empty,
                IsActive = request.StatusHabitDisciplineIsActive,
            };

            var created = await _habitDisciplineRepository.AddAsync(discipline);

            return MapToDto(created, request.IdHabitCategory);
        }
        catch (AppError)
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
                    Message = "An unexpected error occurred while creating the habit discipline.",
                    Details = ex.Message,
                }
            );
        }
    }

    private static HabitDisciplineDetailResponseDto MapToDto(
        HabitDiscipline discipline,
        int? categoryId = null)
    {
        return new HabitDisciplineDetailResponseDto
        {
            IdHabitDiscipline = discipline.Id,
            IdHabitCategory = categoryId ?? discipline.Category.Id,
            DscHabitDisciplineName = discipline.Name,
            DscHabitDisciplineDescription = discipline.Description,
            StatusHabitDisciplineIsActive = discipline.IsActive,
        };
    }
}
