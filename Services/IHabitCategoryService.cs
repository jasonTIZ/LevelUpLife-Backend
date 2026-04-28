using LevelUpLifeBackend.DTOs.Responses;

namespace LevelUpLifeBackend.Services;

public interface IHabitCategoryService
{
    Task<PagedResultDto<HabitCategoryResponseDto>> GetActiveHabitCategoriesPaginatedAsync(
        int pageNumber,
        int pageSize
    );
}
