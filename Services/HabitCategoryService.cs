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
    > GetActiveHabitCategoriesPaginatedAsync(int pageNumber, int pageSize)
    {
        var (habitCategories, totalCount) =
            await _habitCategoryRepository.GetActiveHabitCategoriesPaginatedAsync(
                pageNumber,
                pageSize
            );

        var dtoList = habitCategories.Select(h => new HabitCategoryResponseDto
        {
            Id = h.Id,
            Name = h.Name,
            Description = h.Description,
            IsActive = h.IsActive,
        });

        int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedResultDto<HabitCategoryResponseDto>
        {
            Items = dtoList,
            TotalRecords = totalCount,
            CurrentPage = pageNumber,
            PageSize = pageSize,
        };
    }
}
