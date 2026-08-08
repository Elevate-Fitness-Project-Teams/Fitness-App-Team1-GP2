using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ProgressTrackingService.Common.Redis;
using ProgressTrackingService.Domain.Entities;
using ProgressTrackingService.Features.Progress.WorkoutLog.Command;
using ProgressTrackingService.Infrastructure.Persistence.Context;

namespace ProgressTrackingService.Features.Progress.WorkoutLog.Services;

public class UserStatisticService(AppDbContext appDbContext, RedisServices<UserStatistic> redisServices)
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly RedisServices<UserStatistic> _redisServices = redisServices;
    
    public async Task UpdateUserStatistics(IDbContextTransaction transaction, WorkoutLogCommand request, CancellationToken cancellationToken)
    {
        // Get User Statistic
        var userStatistic = await _appDbContext.UserStatistics
            .AsTracking()
            .FirstOrDefaultAsync(x => x.UserId == request.UserId, cancellationToken);
        
        if (userStatistic == null)
        {
            userStatistic = new UserStatistic
            {
                UserId = request.UserId,
                TotalWorkouts = 1,
                TotalCaloriesBurned = request.Request.CaloriesBurned,
                TotalWeightLost = 0,
                UpdatedAt = DateTimeOffset.UtcNow
            };
            
            await _appDbContext.UserStatistics.AddAsync(userStatistic, cancellationToken);
        }
        else
        {
            userStatistic.TotalWorkouts++;
            userStatistic.TotalCaloriesBurned += request.Request.CaloriesBurned;
            userStatistic.UpdatedAt = DateTimeOffset.UtcNow;
            
            _appDbContext.UserStatistics.Update(userStatistic);
        }
        
        await _appDbContext.SaveChangesAsync(cancellationToken);
        await transaction.CreateSavepointAsync("AfterUserStatisticsUpdate", cancellationToken);
        
        // Redis Cache
        // Remove, If the user key placed from ViewProgressUserStats
        await _redisServices.RemoveRedisCacheAsync($"user_statistic:{request.UserId}", cancellationToken);
        
        // Remove, If the user key placed from ViewUserProgress
        await _redisServices.RemoveRedisCacheAsync($"user_progress:{request.UserId}", cancellationToken);
    }
}