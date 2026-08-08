using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ProgressTrackingService.Common.Redis;
using ProgressTrackingService.Domain.Entities;
using ProgressTrackingService.Infrastructure.Persistence.Context;

namespace ProgressTrackingService.Features.Progress.WorkoutLog.Services;

public record UnlockedAchievementResult(bool IsUnlocked, string AchievementName, string Description);

public class AchievementService(AppDbContext appDbContext, RedisServices<UnlockedAchievementResult> redisServices)
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly RedisServices<UnlockedAchievementResult> _redisServices = redisServices;
    
    public async Task<UnlockedAchievementResult> CheckAndUnlockAchievements(IDbContextTransaction transaction, int userId, CancellationToken cancellationToken)
    {
        // Get User Statistic
        var userStatistics = await _appDbContext.UserStatistics
            .AsTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        if (userStatistics == null) return new UnlockedAchievementResult(false, string.Empty, string.Empty);

        if (userStatistics.TotalWorkouts > 0)
        {
            int[] allowedMilestones = [1, 5, 10, 25, 50, 100, 250, 500];
            if (!allowedMilestones.Contains(userStatistics.TotalWorkouts))
                return new UnlockedAchievementResult(false, string.Empty, string.Empty);
            
            // Get Achievement
            var achievement = await _appDbContext.Achievements
                .FirstOrDefaultAsync(a => a.Name == $"{userStatistics.TotalWorkouts}Workout", cancellationToken);

            if (achievement == null)
            {
                achievement = new Achievement
                {
                    Name = $"{userStatistics.TotalWorkouts}Workout",
                    Description = $"Congratulations on completing your {userStatistics.TotalWorkouts} workout!",
                    IconUrl = $"/icons/{userStatistics.TotalWorkouts}_workout.png"
                };
                
                await _appDbContext.Achievements.AddAsync(achievement, cancellationToken);
                await _appDbContext.SaveChangesAsync(cancellationToken);
            }
            
            // alreadyUnlocked
            // Get User Achievement
            var userAchievement = await _appDbContext.UserAchievements
                .AnyAsync(ua => ua.UserId == userId && ua.AchievementId == achievement.Id, cancellationToken);

            if (userAchievement) return new UnlockedAchievementResult(false, string.Empty, string.Empty);
            
            var newAchievement = new UserAchievement
            {
                UserId = userId,
                AchievementId = achievement.Id,
                AchievedAt = DateTimeOffset.UtcNow
            };

            await _appDbContext.UserAchievements.AddAsync(newAchievement, cancellationToken);
            await _appDbContext.SaveChangesAsync(cancellationToken);
                
            await transaction.CreateSavepointAsync("AfterUserAchievementsInsert", cancellationToken);

            // Redis Cache
            // Remove, If the user key placed from ViewAchievementsUser
            await _redisServices.RemoveRedisCacheAsync($"user_Achievements:{userId}", cancellationToken);
            
            return new UnlockedAchievementResult(true, achievement.Name, achievement.Description);
        }

        return new UnlockedAchievementResult(false, string.Empty, string.Empty);
    }
}