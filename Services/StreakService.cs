using LevelUpLifeBackend.Data;
using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.DTOs.Responses;
using LevelUpLifeBackend.Infrastructure.Configuration;
using LevelUpLifeBackend.Infrastructure.Errors;
using LevelUpLifeBackend.Mappers;
using LevelUpLifeBackend.Models;
using LevelUpLifeBackend.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LevelUpLifeBackend.Services;

public class StreakService : IStreakService
{
    private readonly AppDbContext _context;
    private readonly IStreakLogRepository _streakLogRepository;
    private readonly IPlayerEventRepository _playerEventRepository;
    private readonly StreakProtectionOptions _options;

    public StreakService(
        AppDbContext context,
        IStreakLogRepository streakLogRepository,
        IPlayerEventRepository playerEventRepository,
        IOptions<StreakProtectionOptions> options)
    {
        _context = context;
        _streakLogRepository = streakLogRepository;
        _playerEventRepository = playerEventRepository;
        _options = options.Value;
    }

    public async Task<ActivateStreakProtectionResponseDto> ActivateProtectionAsync(
        int userId,
        ActivateStreakProtectionRequestDto request)
    {
        var player = await _context.PlayerUsers
            .FirstOrDefaultAsync(p => p.Id == userId && p.IsActive);

        if (player is null)
        {
            throw new NotFoundError(
                new ErrorResponse
                {
                    Code = 404,
                    Message = "Player not found",
                    Details = $"Player with id {userId} was not found or is inactive.",
                }
            );
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var existingLog = await _streakLogRepository.GetByPlayerAndDateAsync(userId, today);

        if (existingLog?.CompletionRecorded == true)
        {
            throw new StreakError(
                new ErrorResponse
                {
                    Code = 400,
                    Message = "Streak protection cannot be activated.",
                    Details = "The player already completed a task today.",
                },
                StreakFailureKind.ProtectionAlreadyActive
            );
        }

        if (existingLog?.ProtectionUsed == true)
        {
            throw new StreakError(
                new ErrorResponse
                {
                    Code = 400,
                    Message = "Streak protection already active.",
                    Details = "Protection is already registered for today.",
                },
                StreakFailureKind.ProtectionAlreadyActive
            );
        }

        var usedThisMonth = await _streakLogRepository.CountProtectionsInMonthAsync(
            userId,
            today.Year,
            today.Month
        );

        if (usedThisMonth >= _options.MaxPerMonth)
        {
            throw new StreakError(
                new ErrorResponse
                {
                    Code = 403,
                    Message = "Monthly streak protection limit exceeded.",
                    Details = $"Maximum {_options.MaxPerMonth} protections per calendar month.",
                },
                StreakFailureKind.ProtectionLimitExceeded
            );
        }

        if (existingLog is not null)
        {
            existingLog.ProtectionUsed = true;
            existingLog.ProtectionType = request.Type;
            await _streakLogRepository.UpdateAsync(existingLog);
        }
        else
        {
            var protectionLog = PlayerProgressMapper.ToProtectionStreakLog(player, today, request.Type);
            await _streakLogRepository.AddAsync(protectionLog);
        }

        await _playerEventRepository.AddAsync(new PlayerEvent
        {
            PlayerUserId = userId,
            EventType = PlayerEventType.STREAK_PROTECTION_USED,
            PayloadJson = $"{{\"type\":\"{request.Type}\",\"date\":\"{today:yyyy-MM-dd}\"}}",
            CreatedAt = DateTime.UtcNow,
        });

        var remaining = Math.Max(0, _options.MaxPerMonth - usedThisMonth - 1);

        return new ActivateStreakProtectionResponseDto
        {
            Success = true,
            Message = "Streak protection activated for today.",
            RemainingProtectionsThisMonth = remaining,
        };
    }
}
