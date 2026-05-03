using LevelUpLifeBackend.Data;
using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.DTOs.Responses;
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
                var task = await _habitTaskRepository.AddAsync(new HabitTask { HabitId = habit.Id });
                var criteria = await _repetitionCriteriaRepository.AddAsync(
                    RepetitionCriteriaMapper.ToEntity(task.Id, taskDto.RepetitionCriteria!)
                );
                taskResponses.Add(new HabitTaskResponseDto
                {
                    Id = task.Id,
                    RepetitionCriteria = RepetitionCriteriaMapper.ToResponse(criteria),
                });
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

    public async Task<HabitResponseDto?> GetByIdAsync(int id)
    {
        var habit = await _habitRepository.GetByIdAsync(id);
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
        var existingHabit = await _habitRepository.GetByIdAsync(dto.Id);

        if(existingHabit == null) return null;

        HabitMapper.UpdateEntity(dto, existingHabit);

        await _habitRepository.UpdateHabitAsync(existingHabit);

        var updatedHabit = await _habitRepository.GetByIdAsync(dto.Id);
        return HabitMapper.ToResponse(updatedHabit!);
    }
}
