using LevelUpLifeBackend.DTOs.Responses;
using LevelUpLifeBackend.Mappers;
using LevelUpLifeBackend.Repositories;

namespace LevelUpLifeBackend.Services;

public class HabitCategoryService : IHabitCategoryService
{
    private readonly IHabitCategoryRepository _habitCategoryRepository;

    public HabitCategoryService(IHabitCategoryRepository habitCategoryRepository)
    {
        _habitCategoryRepository = habitCategoryRepository;
    }

    public async Task<
        PagedResultDto<HabitCategoryResponseDto>
    > GetActiveHabitCategoriesPaginatedAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        int? userId = null
    )
    {
        var (habitCategories, totalCount) =
            await _habitCategoryRepository.GetActiveHabitCategoriesPaginatedAsync(
                pageNumber,
                pageSize,
                search,
                userId
            );

        var dtoList = habitCategories.Select(h => new HabitCategoryResponseDto
        {
            Id = h.Category.Id,
            Name = h.Category.Name,
            Description = h.Category.Description,
            ImageUrl = null,
            HabitsCount = h.HabitsCount,
            IsActive = h.Category.IsActive,
        });

        int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedResultDto<HabitCategoryResponseDto>
        {
            Items = dtoList,
            TotalRecords = totalCount,
            TotalPages = totalPages,
            CurrentPage = pageNumber,
            PageSize = pageSize,
        };
    }
}
