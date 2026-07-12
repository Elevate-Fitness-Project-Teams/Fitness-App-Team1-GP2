using MediatR;
using MassTransit;
using ProgressTrackingService.Common.Response;
using ProgressTrackingService.Features.Progress.WorkoutLog.Dto;
using ProgressTrackingService.Infrastructure.Persistence.Context;
using ProgressTrackingService.Features.Progress.WorkoutLog.Events;
using ProgressTrackingService.Features.Progress.WorkoutLog.Command;
using ProgressTrackingService.Features.Progress.WorkoutLog.Services;

namespace ProgressTrackingService.Features.Progress.WorkoutLog.Handler;

public class WorkoutLogCommandHandler (
    AppDbContext appDbContext,
    WorkoutLogService workoutLogService,
    StreakService streakService,
    UserStatisticService userStatisticService,
    AchievementService achievementService,
    IPublishEndpoint publishEndpoint): IRequestHandler<WorkoutLogCommand, ResponseResult<WorkoutLogDto>>
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly WorkoutLogService _workoutLogService = workoutLogService;
    private readonly StreakService _streakService = streakService;
    private readonly UserStatisticService _userStatisticService = userStatisticService;
    private readonly AchievementService _achievementService = achievementService;
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
    
    public async Task<ResponseResult<WorkoutLogDto>> Handle(WorkoutLogCommand request, CancellationToken cancellationToken)
    {
        // Step 1: Begin Transaction
        await using var transaction = await _appDbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // Step 2: Insert WorkoutLog - Step 3: Insert WorkoutLogExercises
            int logId = await _workoutLogService.InsertWorkoutLogAndExercises(transaction, request, cancellationToken); // Return WorkOutId
            
            // Step 4: Update Streak
            var streakResult = await _streakService.UpdateUserStreak(transaction, request, cancellationToken);
            
            // Step 5: Update UserStatistics
            await _userStatisticService.UpdateUserStatistics(transaction, request, cancellationToken);
            
            // Step 7: Check Achievements - Step 8: Insert UserAchievements
            var achievementResult = await _achievementService.CheckAndUnlockAchievements(transaction, request.UserId, cancellationToken);
            
            await transaction.CommitAsync(cancellationToken);
            
            // Step 9: Publish WorkoutLogged
            await _publishEndpoint.Publish(new WorkoutLoggedEvent
            {
                UserId = request.UserId,
                WorkoutLogId = logId,
                CaloriesBurned = request.Request.CaloriesBurned,
                CaloriesBurnedAt = DateTimeOffset.UtcNow
            }, cancellationToken);
            
            // Step 10: Publish AchievementEarned
            if (achievementResult.IsUnlocked)
            {
                await _publishEndpoint.Publish(new AchievementEarnedEvent
                {
                    UserId = request.UserId,
                    AchievementName = achievementResult.AchievementName,
                    Description = achievementResult.Description,
                    EarnedAt = DateTimeOffset.UtcNow
                }, cancellationToken);
            }
            
            return ResponseResult<WorkoutLogDto>.Success(new WorkoutLogDto()
            {
                WorkoutLogLogId = logId,
                CurrentStreak = streakResult.CurrentStreak,
                StreakUpdated = streakResult.IsUpdated
            }, "Workout logged successfully", StatusCode.Success);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(cancellationToken);
            Console.WriteLine(e);
            throw;
        }
    }
}