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
    private readonly ITimerCriteriaRepository _timerCriteriaRepository;
    private readonly IStreakLogRepository _streakLogRepository;
    private readonly IPlayerEventRepository _playerEventRepository;
    private readonly AppDbContext _context;

    public HabitService(
        IHabitRepository habitRepository,
        IHabitTaskRepository habitTaskRepository,
        IRepetitionCriteriaRepository repetitionCriteriaRepository,
        ITimerCriteriaRepository timerCriteriaRepository,
        IStreakLogRepository streakLogRepository,
        IPlayerEventRepository playerEventRepository,
        AppDbContext context)
    {
        _habitRepository = habitRepository;
        _habitTaskRepository = habitTaskRepository;
        _repetitionCriteriaRepository = repetitionCriteriaRepository;
        _timerCriteriaRepository = timerCriteriaRepository;
        _streakLogRepository = streakLogRepository;
        _playerEventRepository = playerEventRepository;
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

                TimerCriteria? timerCriteria = null;
                if (taskDto.CompletionCriteria == TaskCompletionCriteria.TIMER)
                {
                    timerCriteria = await _timerCriteriaRepository.AddAsync(
                        TimerCriteriaMapper.ToEntity(task.Id, taskDto.TimerCriteria!)
                    );
                }

                var taskResponse = HabitTaskMapper.ToResponse(task);
                if (criteria is not null)
                {
                    taskResponse.RepetitionCriteria = RepetitionCriteriaMapper.ToResponse(criteria);
                }
                if (timerCriteria is not null)
                {
                    taskResponse.TimerCriteria = TimerCriteriaMapper.ToResponse(timerCriteria);
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

            TimerCriteria? timerCriteria = null;
            if (request.CompletionCriteria == TaskCompletionCriteria.TIMER)
            {
                timerCriteria = await _timerCriteriaRepository.AddAsync(
                    TimerCriteriaMapper.ToEntity(task.Id, request.TimerCriteria!)
                );
            }

            await transaction.CommitAsync();

            var created = await _habitTaskRepository.GetByIdWithCriteriaAsync(task.Id);
            var response = HabitTaskMapper.ToResponse(created ?? task);
            if (criteria is not null)
            {
                response.RepetitionCriteria = RepetitionCriteriaMapper.ToResponse(criteria);
            }
            if (timerCriteria is not null)
            {
                response.TimerCriteria = TimerCriteriaMapper.ToResponse(timerCriteria);
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

        HabitTaskMapper.ApplyStandaloneRequest(request, task, task.Habit.Discipline.Id);

        if (request.CompletionCriteria == TaskCompletionCriteria.REPETITIONS)
        {
            if (task.RepetitionCriteria is null)
            {
                task.RepetitionCriteria = RepetitionCriteriaMapper.ToEntity(
                    task.Id,
                    request.RepetitionCriteria!
                );
            }
            else
            {
                RepetitionCriteriaMapper.UpdateEntity(task.RepetitionCriteria, request.RepetitionCriteria!);
            }
        }

        if (request.CompletionCriteria == TaskCompletionCriteria.TIMER
            && request.TimerCriteria is not null
            && task.TimerCriteria is not null)
        {
            TimerCriteriaMapper.UpdateEntity(task.TimerCriteria, request.TimerCriteria);
        }

        await _habitTaskRepository.UpdateWithRepetitionCriteriaAsync(task);

        return HabitTaskMapper.ToResponse(task);
    }

    public async Task DeactivateTaskAsync(int taskId, int userId)
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

        if (!task.Habit.IsActive)
        {
            throw new NotFoundError(
                new ErrorResponse
                {
                    Code = 404,
                    Message = "Habit not found",
                    Details = $"Habit with id {task.HabitId} does not exist or is inactive.",
                }
            );
        }

        if (task.IsActive)
        {
            task.IsActive = false;
            await _habitTaskRepository.UpdateAsync(task);
        }
    }

    public async Task<CompleteHabitTaskResponseDto> CompleteTaskAsync(
        int taskId,
        int userId,
        CompleteHabitTaskRequestDto request)
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

        if (!task.Habit.IsActive)
        {
            throw new NotFoundError(
                new ErrorResponse
                {
                    Code = 404,
                    Message = "Habit not found",
                    Details = $"Habit with id {task.HabitId} does not exist or is inactive.",
                }
            );
        }

        await EnsureCompletionRequirementsMetAsync(task, request.CompletedAt);

        var player = task.Habit.User;
        var xpEarned = PlayerProgressMapper.CalculateXpEarned(task);
        var completionDate = DateOnly.FromDateTime(request.CompletedAt.ToUniversalTime());

        task.IsCompleted = true;
        task.EarnedXpSnapshot = xpEarned;
        var previousLevel = player.Level;
        player.ExperiencePoints += xpEarned;
        player.Level = PlayerProgressMapper.CalculateLevel(player.ExperiencePoints);

        var streakResult = await ApplyStreakUpdateAsync(player, completionDate);

        await _habitTaskRepository.CompleteTaskAsync(task, streakResult.Log);

        await RecordCompletionEventsAsync(
            player.Id,
            streakResult,
            previousLevel,
            player.Level,
            completionDate
        );

        return PlayerProgressMapper.ToCompleteResponse(xpEarned, player.Level, streakResult.Updated);
    }

    private async Task RecordCompletionEventsAsync(
        int playerUserId,
        StreakUpdateResult streakResult,
        int previousLevel,
        int newLevel,
        DateOnly completionDate)
    {
        if (streakResult.Updated && streakResult.WasReset)
        {
            await _playerEventRepository.AddAsync(new PlayerEvent
            {
                PlayerUserId = playerUserId,
                EventType = PlayerEventType.STREAK_RESET,
                PayloadJson = $"{{\"date\":\"{completionDate:yyyy-MM-dd}\",\"newStreak\":1}}",
                CreatedAt = DateTime.UtcNow,
            });
        }
        else if (streakResult.Updated && streakResult.ContinuedViaProtection)
        {
            await _playerEventRepository.AddAsync(new PlayerEvent
            {
                PlayerUserId = playerUserId,
                EventType = PlayerEventType.STREAK_CONTINUED,
                PayloadJson = $"{{\"date\":\"{completionDate:yyyy-MM-dd}\",\"streak\":{streakResult.NewStreakCount}}}",
                CreatedAt = DateTime.UtcNow,
            });
        }

        if (newLevel > previousLevel)
        {
            await _playerEventRepository.AddAsync(new PlayerEvent
            {
                PlayerUserId = playerUserId,
                EventType = PlayerEventType.LEVEL_UP,
                PayloadJson = $"{{\"previousLevel\":{previousLevel},\"newLevel\":{newLevel}}}",
                CreatedAt = DateTime.UtcNow,
            });
        }
    }

    private sealed record StreakUpdateResult(
        bool Updated,
        StreakLog? Log,
        bool WasReset,
        bool ContinuedViaProtection,
        int NewStreakCount
    );

    private async Task EnsureCompletionRequirementsMetAsync(HabitTask task, DateTime completedAt)
    {
        if (!task.IsActive)
        {
            ThrowCompletionRequirementsNotMet("The habit task is inactive and cannot be completed.");
        }

        if (task.IsCompleted)
        {
            ThrowCompletionRequirementsNotMet("The habit task has already been completed.");
        }

        var completionDate = DateOnly.FromDateTime(completedAt.ToUniversalTime());
        if (completionDate < task.StartDate)
        {
            ThrowCompletionRequirementsNotMet(
                "The completion date cannot be earlier than the task start date."
            );
        }

        switch (task.CompletionCriteria)
        {
            case TaskCompletionCriteria.EVIDENCE:
                var evidences = await _habitTaskRepository.GetEvidencesByTaskIdAsync(task.Id);
                if (!evidences.Any())
                {
                    ThrowCompletionRequirementsNotMet(
                        "Evidence must be uploaded before completing a task with EVIDENCE criteria."
                    );
                }
                break;

            case TaskCompletionCriteria.REPETITIONS:
                if (task.RepetitionCriteria is null || !task.RepetitionCriteria.StatusRepetitionsCriteriaIsActive)
                {
                    ThrowCompletionRequirementsNotMet(
                        "Active repetition criteria are required before completing this task."
                    );
                }
                break;

            case TaskCompletionCriteria.TIMER:
                if (task.TimerCriteria is null || !task.TimerCriteria.StatusTimerCriteriaIsActive)
                {
                    ThrowCompletionRequirementsNotMet(
                        "Active timer criteria are required before completing this task."
                    );
                }
                break;
        }
    }

    private async Task<StreakUpdateResult> ApplyStreakUpdateAsync(PlayerUser player, DateOnly completionDate)
    {
        var existingLog = await _streakLogRepository.GetByPlayerAndDateAsync(player.Id, completionDate);
        if (existingLog?.CompletionRecorded == true)
        {
            return new StreakUpdateResult(false, null, false, false, player.DaysStreak);
        }

        var lastCompletion = await _streakLogRepository.GetLastCompletionBeforeAsync(
            player.Id,
            completionDate
        );

        var dayGap = lastCompletion is null
            ? 0
            : completionDate.DayNumber - lastCompletion.LogDate.DayNumber;

        var gapHasProtection = lastCompletion is not null
            && dayGap > 1
            && await _streakLogRepository.HasProtectionInGapAsync(
                player.Id,
                lastCompletion.LogDate,
                completionDate
            );

        var (newStreak, wasReset) = StreakCalculator.ComputeStreakCount(
            lastCompletion,
            completionDate,
            gapHasProtection
        );

        player.DaysStreak = Math.Max(1, newStreak);
        var continuedViaProtection = gapHasProtection && !wasReset && dayGap > 1;

        StreakLog log;
        if (existingLog is not null)
        {
            existingLog.StreakCount = player.DaysStreak;
            existingLog.CompletionRecorded = true;
            log = existingLog;
        }
        else
        {
            log = PlayerProgressMapper.ToStreakLog(player, completionDate);
        }

        return new StreakUpdateResult(true, log, wasReset, continuedViaProtection, player.DaysStreak);
    }

    private static void ThrowCompletionRequirementsNotMet(string details)
    {
        throw new TaskError(
            new ErrorResponse
            {
                Code = 400,
                Message = "Completion requirements not met.",
                Details = details,
            }
        );
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
                .ThenInclude(t => t.RepetitionCriteria)
            .Include(h => h.Tasks)
                .ThenInclude(t => t.TimerCriteria)
            .FirstOrDefaultAsync(h => h.Id == dto.Id);
        if (existingHabit is null) return null;

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            HabitMapper.UpdateEntity(dto, existingHabit);

            if (dto.Tasks is not null)
            {
                foreach (var taskDto in dto.Tasks)
                {
                    var task = existingHabit.Tasks.FirstOrDefault(t => t.Id == taskDto.TaskId);
                    if (task is null) continue;

                    HabitTaskMapper.ApplyNestedUpdateRequest(
                        taskDto,
                        task,
                        existingHabit.Discipline.Id
                    );

                    if (taskDto.RepetitionCriteria is not null)
                    {
                        var criteria = await _repetitionCriteriaRepository.GetByTaskIdAsync(taskDto.TaskId);
                        if (criteria is not null)
                        {
                            RepetitionCriteriaMapper.UpdateEntity(criteria, taskDto.RepetitionCriteria);
                        }
                    }

                    if (taskDto.TimerCriteria is not null && task.TimerCriteria is not null)
                    {
                        TimerCriteriaMapper.UpdateEntity(task.TimerCriteria, taskDto.TimerCriteria);
                    }
                }
            }

            _context.Entry(existingHabit).Property("DisciplineId").CurrentValue = dto.DisciplineId;

            await _habitRepository.UpdateHabitAsync(existingHabit);

            await transaction.CommitAsync();

            var updatedHabit = await _context.Habits
                .AsNoTracking()
                .Include(h => h.Discipline).ThenInclude(d => d.Category)
                .Include(h => h.User)
                .Include(h => h.Tasks)
                    .ThenInclude(t => t.RepetitionCriteria)
                .Include(h => h.Tasks)
                    .ThenInclude(t => t.TimerCriteria)
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
