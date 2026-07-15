using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ProgressTrackingService.Common.Redis;
using ProgressTrackingService.Features.Progress.WorkoutLog.Command;
using ProgressTrackingService.Infrastructure.Persistence.Context;
using Streak =  ProgressTrackingService.Domain.Entities.Streak;

namespace ProgressTrackingService.Features.Progress.WorkoutLog.Services;

public class StreakService (AppDbContext appDbContext, RedisServices<Streak> redisServices)
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly RedisServices<Streak> _redisServices = redisServices;
    
    public async Task<(int CurrentStreak, bool IsUpdated)> UpdateUserStreak(IDbContextTransaction transaction, WorkoutLogCommand request, CancellationToken cancellationToken)
    {
        var streak = await _appDbContext.Streaks
            .AsTracking()
            .FirstOrDefaultAsync(x => x.UserId == request.UserId, cancellationToken);

        bool isUpdated;

        if (streak is null)
        {
            streak = new Streak
            {
                UserId = request.UserId,
                CurrentStreak = 1,
                LongestStreak = 1,
                LastWorkoutDate = request.Request.CompletedAt
            };
            await _appDbContext.Streaks.AddAsync(streak, cancellationToken);
            isUpdated = true;
        }
        else
        {
            streak.CurrentStreak++;
            streak.LastWorkoutDate = request.Request.CompletedAt;
            isUpdated = true;

            if (streak.CurrentStreak > streak.LongestStreak)
            {
                streak.LongestStreak = streak.CurrentStreak;
            }

            _appDbContext.Streaks.Update(streak);
        }

        await _appDbContext.SaveChangesAsync(cancellationToken);
        await transaction.CreateSavepointAsync("AfterStreakUpdate", cancellationToken);

        // Redis Cache
        // Remove, If the user key placed from ViewProgressUserStats
        await _redisServices.RemoveRedisCacheAsync($"user_statistic:{request.UserId}", cancellationToken);
        
        return (streak.CurrentStreak, isUpdated);
    }
}