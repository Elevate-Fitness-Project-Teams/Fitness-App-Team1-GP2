using MediatR;
using Microsoft.EntityFrameworkCore;
using ProgressTrackingService.Common.Redis;
using ProgressTrackingService.Common.Response;
using ProgressTrackingService.Infrastructure.Persistence.Context;
using ProgressTrackingService.Features.Progress.Dashboard.ViewProgressUserStats.Dto;
using ProgressTrackingService.Features.Progress.Dashboard.ViewProgressUserStats.Query;


namespace ProgressTrackingService.Features.Progress.Dashboard.ViewProgressUserStats.Handler;

public class ViewProgressUserStatsQueryHandler(
    AppDbContext appDbContext,
    RedisServices<ViewProgressUserStatsDto> redisServices): IRequestHandler<ViewProgressUserStatsQuery, ResponseResult<ViewProgressUserStatsDto>>
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly RedisServices<ViewProgressUserStatsDto> _redisServices = redisServices;
    
    public async Task<ResponseResult<ViewProgressUserStatsDto>> Handle(ViewProgressUserStatsQuery request, CancellationToken cancellationToken)
    {
        var cacheResult = await _redisServices.GetRedisCacheAsync($"user_statistic:{request.UserId}", cancellationToken);
        if (cacheResult.IsSuccess) return ResponseResult<ViewProgressUserStatsDto>.Success(cacheResult.Data,"",StatusCode.Success);
        
        // Step 1: totalWorkouts --> WorkOutLog
        var totalWorkouts = await _appDbContext.WorkoutLogs
            .CountAsync(w => w.UserId == request.UserId, cancellationToken);
        
        // Step 2: totalCaloriesBurned - totalWeightLost --> UserStatistic
        var userStatistic = await _appDbContext.UserStatistics
            .Where(u => u.UserId == request.UserId)
            .OrderByDescending(u => u.UpdatedAt)
            .Select(u => new ViewProgressUserStatsDto()
            {
                TotalCaloriesBurned = u.TotalCaloriesBurned,
                TotalWeightLost = u.TotalWeightLost
            })
            .FirstOrDefaultAsync(cancellationToken);
        
        if (userStatistic == null )
            return ResponseResult<ViewProgressUserStatsDto>.Failure(StatusCode.NotFound, "User statistics not found.");
        
        // Step 3: currentStreak - longestStreak --> Streaks
        var userStreaks = await _appDbContext.Streaks
            .Where(s => s.UserId == request.UserId)
            .OrderByDescending(u => u.LastWorkoutDate)
            .Select(s => new ViewProgressUserStatsDto()
            {
                CurrentStreak = s.CurrentStreak,
                LongestStreak = s.LongestStreak
            })
            .FirstOrDefaultAsync(cancellationToken);
        
        if (userStreaks == null)
            return ResponseResult<ViewProgressUserStatsDto>.Failure(StatusCode.NotFound, "User streak information not found.");

        var viewProgressUserStatsDto = new ViewProgressUserStatsDto()
        {
            UserId = request.UserId,
            TotalWorkouts = totalWorkouts,
            TotalCaloriesBurned = userStatistic.TotalCaloriesBurned,
            TotalWeightLost = userStatistic.TotalWeightLost,
            CurrentStreak = userStreaks.CurrentStreak,
            LongestStreak = userStreaks.LongestStreak
        };

        // Redis Cache
        // Make sure to remove the key, If insert new records in all steps (Workout Logs - User Statistics - Streaks)
        await _redisServices.SetRedisCacheAsync($"user_statistic:{request.UserId}", viewProgressUserStatsDto, 1, 5, cancellationToken);
        
        return ResponseResult<ViewProgressUserStatsDto>.Success(viewProgressUserStatsDto, "User stats retrieved successfully.", StatusCode.Success);
    }
}