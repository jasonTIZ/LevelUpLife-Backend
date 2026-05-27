using LevelUpLifeBackend.Data;
using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.DTOs.Responses;
using LevelUpLifeBackend.Infrastructure.Errors;
using LevelUpLifeBackend.Mappers;
using LevelUpLifeBackend.Models;
using LevelUpLifeBackend.Repositories;

namespace LevelUpLifeBackend.Services;

public class HabitService : IHabitService
{
    private readonly IHabitRepository _habitRepository;
    private readonly IHabitTaskRepository _habitTaskRepository;
    private readonly IRepetitionCriteriaRepository _repetitionCriteriaRepository;
    private readonly AppDbContext _context;

    public HabitService(
        IHabitRepository habitRepository,
        IHabitTaskRepository habitTaskRepository,
        IRepetitionCriteriaRepository repetitionCriteriaRepository,
        AppDbContext context)
    {
        _habitRepository = habitRepository;
        _habitTaskRepository = habitTaskRepository;
        _repetitionCriteriaRepository = repetitionCriteriaRepository;
        _context = context;
    }

    public async Task<HabitResponseDto> CreateAsync(CreateHabitRequestDto request)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var habit = await _habitRepository.AddAsync(HabitMapper.ToEntity(request));

            var taskResponses = new List<HabitTaskResponseDto>();
            foreach (var taskDto in request.Tasks)
            {
                var task = await _habitTaskRepository.AddAsync(
                    HabitTaskMapper.ToEntity(taskDto, habit.Id, habit.Discipline.Id)
                );
                RepetitionCriteria? criteria = null;
                if (taskDto.CompletionCriteria == TaskCompletionCriteria.REPETITIONS)
                {
                    criteria = await _repetitionCriteriaRepository.AddAsync(
                        RepetitionCriteriaMapper.ToEntity(task.Id, taskDto.RepetitionCriteria!)
                    );
                }
                var taskResponse = HabitTaskMapper.ToResponse(task);
                if (criteria is not null)
                {
                    taskResponse.RepetitionCriteria = RepetitionCriteriaMapper.ToResponse(criteria);
                }
                taskResponses.Add(taskResponse);
            }

            await transaction.CommitAsync();

            var response = HabitMapper.ToResponse(habit);
            response.Tasks = taskResponses;
            return response;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<HabitTaskResponseDto> CreateTaskAsync(
        CreateStandaloneHabitTaskRequestDto request,
        int userId
    )
    {
        var habit = await _habitRepository.GetByIdAsync(request.HabitId);
        if (habit is null || !habit.IsActive)
        {
            throw new NotFoundError(
                new ErrorResponse
                {
                    Code = 404,
                    Message = "Habit not found",
                    Details = $"Habit with id {request.HabitId} does not exist or is inactive.",
                }
            );
        }

        if (habit.User.Id != userId)
        {
            throw new AuthError(
                403,
                new ErrorResponse
                {
                    Code = 403,
                    Message = "Forbidden",
                    Details = "You can only create tasks for your own habits.",
                },
                AuthFailureKind.Forbidden
            );
        }

        if (await _habitTaskRepository.ExistsActiveByHabitIdAsync(request.HabitId))
        {
            throw new ConflictError(
                new ErrorResponse
                {
                    Code = 409,
                    Message = "TASK_ALREADY_EXISTS",
                    Details = $"Habit {request.HabitId} already has an active task.",
                }
            );
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var task = await _habitTaskRepository.AddAsync(
                HabitTaskMapper.ToEntity(request, habit.Discipline.Id)
            );

            RepetitionCriteria? criteria = null;
            if (request.CompletionCriteria == TaskCompletionCriteria.REPETITIONS)
            {
                criteria = await _repetitionCriteriaRepository.AddAsync(
                    RepetitionCriteriaMapper.ToEntity(task.Id, request.RepetitionCriteria!)
                );
            }

            await transaction.CommitAsync();

            var created = await _habitTaskRepository.GetByIdWithCriteriaAsync(task.Id);
            var response = HabitTaskMapper.ToResponse(created ?? task);
            if (criteria is not null)
            {
                response.RepetitionCriteria = RepetitionCriteriaMapper.ToResponse(criteria);
            }
            return response;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<HabitResponseDto?> GetByIdAsync(int id, int userId)
    {
        var habit = await _habitRepository.GetByIdAsync(id);
        if (habit is null || habit.User.Id != userId)
        {
            return null;
        }

        return HabitMapper.ToResponse(habit);
    }

    public async Task<PagedResultDto<HabitResponseDto>> GetActiveHabitsPaginatedAsync(
        int pageNumber,
        int pageSize,
        int userId
    )
    {
        var (habits, totalCount) = await _habitRepository.GetActiveHabitsPaginatedAsync(
            pageNumber,
            pageSize,
            userId
        );
        var dtoList = habits.Select(h => new HabitResponseDto
        {
            Id = h.Id,
            Title = h.Title,
            Description = h.Description,
            IsActive = h.IsActive,
            UserName = h.User.UserName,
            DisciplineName = h.Discipline.Name,
            CategoryName = h.Discipline.Category.Name,
        });

        int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedResultDto<HabitResponseDto>
        {
            Items = dtoList,
            TotalRecords = totalCount,
            TotalPages = totalPages,
            CurrentPage = pageNumber,
            PageSize = pageSize,
        };
    }

    public async Task<HabitResponseDto?> UpdateHabitAsync(UpdateHabitRequestDto dto)
    {
        var existingHabit = await _habitRepository.GetByIdAsync(dto.Id);
        if (existingHabit is null) return null;

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            HabitMapper.UpdateEntity(dto, existingHabit);
            await _habitRepository.UpdateHabitAsync(existingHabit);

            if (dto.Tasks is not null)
            {
                foreach (var taskDto in dto.Tasks)
                {
                    if (taskDto.RepetitionCriteria is null) continue;

                    var criteria = await _repetitionCriteriaRepository.GetByTaskIdAsync(taskDto.TaskId);
                    if (criteria is null) continue;

                    RepetitionCriteriaMapper.UpdateEntity(criteria, taskDto.RepetitionCriteria);
                    await _repetitionCriteriaRepository.UpdateAsync(criteria);
                }
            }

            await transaction.CommitAsync();

            var updatedHabit = await _habitRepository.GetByIdAsync(dto.Id);
            return HabitMapper.ToResponse(updatedHabit!);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
