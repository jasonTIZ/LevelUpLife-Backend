using LevelUpLifeBackend.Data;
using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.DTOs.Responses;
using LevelUpLifeBackend.Infrastructure.Errors;
using LevelUpLifeBackend.Mappers;
using LevelUpLifeBackend.Models;
using LevelUpLifeBackend.Repositories;
using Microsoft.EntityFrameworkCore;

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
        var habit = await _habitRepository.GetByIdAsync(request.HabitId, userId);
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
            var task = await _habitTaskRepository.AddAsync(new HabitTask
            {
                HabitId = request.HabitId,
                HabitDisciplineId = request.HabitDisciplineId ?? habit.Discipline.Id,
                Title = request.Title.Trim(),
                Description = request.Description,
                WeekDays = request.WeekDays,
                Difficulty = request.Difficulty!.Value,
                XpValue = request.XpValue ?? 0,
                Frequency = request.Frequency!.Value,
                PeriodLength = request.PeriodLength ?? 1,
                PeriodUnit = request.PeriodUnit ?? TaskPeriodUnit.DAYS,
                StartDate = request.StartDate ?? DateOnly.FromDateTime(DateTime.UtcNow),
                CompletionCriteria = request.CompletionCriteria!.Value,
                Evidence = request.Evidence,
                IsActive = request.IsActive ?? true,
                IsCompleted = false,
            });

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

    public async Task<HabitTaskResponseDto> UpdateTaskAsync(
        int taskId,
        CreateStandaloneHabitTaskRequestDto request,
        int userId
    )
    {
        var task = await _habitTaskRepository.GetTrackedByIdForUserAsync(taskId, userId);
        if (task is null || task.Habit is null)
        {
            throw new NotFoundError(
                new ErrorResponse
                {
                    Code = 404,
                    Message = "Task not found",
                    Details = $"Habit task with id {taskId} was not found for the authenticated user.",
                }
            );
        }

        if (task.HabitId != request.HabitId)
        {
            throw new AppError(
                400,
                new ErrorResponse
                {
                    Code = 400,
                    Message = "Validation failed",
                    Details = "HabitId in the request body must match the task's habit.",
                }
            );
        }

        if (!task.Habit.IsActive)
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

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            HabitTaskMapper.ApplyStandaloneRequest(request, task, task.Habit.Discipline.Id);
            await _habitTaskRepository.UpdateAsync(task);

            RepetitionCriteria? criteria = task.RepetitionCriteria;
            if (request.CompletionCriteria == TaskCompletionCriteria.REPETITIONS)
            {
                if (criteria is null)
                {
                    criteria = await _repetitionCriteriaRepository.AddAsync(
                        RepetitionCriteriaMapper.ToEntity(task.Id, request.RepetitionCriteria!)
                    );
                }
                else
                {
                    RepetitionCriteriaMapper.UpdateEntity(criteria, request.RepetitionCriteria!);
                    criteria = await _repetitionCriteriaRepository.UpdateAsync(criteria);
                }
            }

            await transaction.CommitAsync();

            if (criteria is not null)
            {
                task.RepetitionCriteria = criteria;
            }

            return HabitTaskMapper.ToResponse(task);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<HabitResponseDto?> GetByIdAsync(int id, int userId)
    {
        var habit = await _habitRepository.GetByIdAsync(id, userId);
        return habit is null ? null : HabitMapper.ToResponse(habit);
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
        var existingHabit = await _context.Habits
            .Include(h => h.Discipline).ThenInclude(d => d.Category)
            .Include(h => h.User)
            .Include(h => h.Tasks)
            .FirstOrDefaultAsync(h => h.Id == dto.Id);
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

            var updatedHabit = await _context.Habits
                .AsNoTracking()
                .Include(h => h.Discipline).ThenInclude(d => d.Category)
                .Include(h => h.User)
                .Include(h => h.Tasks)
                .FirstOrDefaultAsync(h => h.Id == dto.Id);
            return HabitMapper.ToResponse(updatedHabit!);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
