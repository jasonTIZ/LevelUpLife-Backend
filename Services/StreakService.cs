using LevelUpLifeBackend.Data;
using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.DTOs.Responses;
using LevelUpLifeBackend.Infrastructure;
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
    private readonly StreakProtectionOptions _options;

    public StreakService(
        AppDbContext context,
        IStreakLogRepository streakLogRepository,
        IOptions<StreakProtectionOptions> options)
    {
        _context = context;
        _streakLogRepository = streakLogRepository;
        _options = options.Value;
    }

    public async Task<ActivateStreakProtectionResponseDto> ActivateProtectionAsync(
        int userId,
        ActivateStreakProtectionRequestDto request)
    {
        if (!StreakProtectionTypeParser.TryParse(request.Type, out var protectionType))
        {
            throw new StreakError(
                new ErrorResponse
                {
                    Code = 400,
                    Message = "Invalid streak protection type.",
                    Details = "Type must be one of: TRABAJO, EVALUACION, EMERGENCIA.",
                },
                StreakFailureKind.InvalidProtectionType
            );
        }

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

        var playerEvent = new PlayerEvent
        {
            PlayerUserId = userId,
            EventType = PlayerEventType.STREAK_PROTECTION_USED,
            PayloadJson = $"{{\"type\":\"{protectionType}\",\"date\":\"{today:yyyy-MM-dd}\"}}",
            CreatedAt = DateTime.UtcNow,
        };

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            if (existingLog is not null)
            {
                existingLog.ProtectionUsed = true;
                existingLog.ProtectionType = protectionType;
                _context.StreakLogs.Update(existingLog);
            }
            else
            {
                var protectionLog = PlayerProgressMapper.ToProtectionStreakLog(
                    player,
                    today,
                    protectionType
                );
                await _context.StreakLogs.AddAsync(protectionLog);
            }

            await _context.PlayerEvents.AddAsync(playerEvent);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

        var remaining = Math.Max(0, _options.MaxPerMonth - usedThisMonth - 1);

        return new ActivateStreakProtectionResponseDto
        {
            Success = true,
            Message = "Streak protection activated for today.",
            RemainingProtectionsThisMonth = remaining,
        };
    }
}
