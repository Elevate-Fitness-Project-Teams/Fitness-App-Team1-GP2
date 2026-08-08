using MediatR;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using ProgressTrackingService.Common.Response;
using ProgressTrackingService.Features.Progress.WorkoutLog.Dto;
using ProgressTrackingService.Infrastructure.Persistence.Context;
using ProgressTrackingService.Features.Progress.WorkoutLog.Events;
using ProgressTrackingService.Features.Progress.WorkoutLog.Command;
using ProgressTrackingService.Features.Progress.WorkoutLog.Request;
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

        // Event
        // "message": {
        //     "sessionId": "c0cde8bd37f24501b18b9938c74ee98a",
        //     "userId": 10,
        //     "workoutId": 1,
        //     "startedAt": "2026-08-07T14:16:59.430317Z",
        //     "eventId": "9521519c-f1aa-4a56-bc8b-53f470abafc8",
        //     "createdAt": "2026-08-07T14:16:59.609675Z"
        // },
        
        try
        {
            // Step 2: 
            var workoutSessionTrackings = await _appDbContext.WorkoutSessionTrackings
                .OrderByDescending(x => x.StartedAt)
                .FirstOrDefaultAsync(x =>  x.UserId == request.UserId, cancellationToken);
            
            if (workoutSessionTrackings?.SessionId == null)
                return ResponseResult<WorkoutLogDto>.Failure(StatusCode.NotFound, "Workout session was not found.");

            if (workoutSessionTrackings.UserId != request.UserId)
                return ResponseResult<WorkoutLogDto>.Failure(StatusCode.Unauthorized, "This workout session does not belong to the current user.");
            
            request.Request.WorkoutId = workoutSessionTrackings.WorkoutId;
            request.Request.SessionId = workoutSessionTrackings.SessionId;
            
            // Step 3: Insert WorkoutLog - Step 3: Insert WorkoutLogExercises
            int logId = await _workoutLogService.InsertWorkoutLogAndExercises(transaction, request, cancellationToken); // Return WorkOutId
            
            // Step 4: Update Streak
            var streakResult = await _streakService.UpdateUserStreak(transaction, request, cancellationToken);
            
            // Step 5: Update UserStatistics
            await _userStatisticService.UpdateUserStatistics(transaction, request, cancellationToken);
            
            // Step 6: Check Achievements - Step 7: Insert UserAchievements
            var achievementResult = await _achievementService.CheckAndUnlockAchievements(transaction, request.UserId, cancellationToken);
            
            // Step 8: Publish WorkoutLogged
            await _publishEndpoint.Publish(new WorkoutLoggedEvent
            {
                UserId = request.UserId,
                WorkoutLogId = logId,
                CaloriesBurned = request.Request.CaloriesBurned,
                CaloriesBurnedAt = DateTimeOffset.UtcNow
            }, cancellationToken);
            
            // Step 9: Publish AchievementEarned
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
            
            // Step 10: Commit
            await transaction.CommitAsync(cancellationToken);
            
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