using LevelUpLifeBackend.DTOs.Responses;
using LevelUpLifeBackend.Models;

namespace LevelUpLifeBackend.Mappers;

public static class HabitCategoryMapper
{
    public static HabitCategoryResponseDto ToResponse(HabitCategory habitCategory)
    {
        return new HabitCategoryResponseDto
        {
            Id = habitCategory.Id,
            Name = habitCategory.Name,
            Description = habitCategory.Description,
            IsActive = habitCategory.IsActive,
        };
    }
}
