using Microsoft.EntityFrameworkCore.Storage;
using ProgressTrackingService.Common.Redis;
using ProgressTrackingService.Domain.Entities;
using ProgressTrackingService.Features.Progress.WorkoutLog.Command;
using ProgressTrackingService.Infrastructure.Persistence.Context;
using workoutLog =  ProgressTrackingService.Domain.Entities.WorkoutLog;

namespace ProgressTrackingService.Features.Progress.WorkoutLog.Services;

public class WorkoutLogService (AppDbContext appDbContext, RedisServices<workoutLog> redisServices)
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly RedisServices<workoutLog> _redisServices = redisServices;
    
    public async Task<int> InsertWorkoutLogAndExercises(IDbContextTransaction transaction, WorkoutLogCommand request, CancellationToken cancellationToken)
    {
        var workOutLog = new workoutLog
        {
            UserId = request.UserId,
            WorkoutId = request.Request.WorkoutId,
            SessionId = request.Request.SessionId,
            CompletedAt = request.Request.CompletedAt,
            DurationInMinutes = request.Request.Duration,
            CaloriesBurned = request.Request.CaloriesBurned,
            DifficultyLevel = request.Request.Difficulty,
            Notes = request.Request.Notes,
            Rating = request.Request.Rating,
            WorkoutLogExercises = request.Request.ExercisesCompleted
                .Select(x => new WorkoutLogExercise
                {
                    ExerciseId = x.ExerciseId,
                    SetsCompleted = x.Sets,
                    RepsCompleted = x.Reps,
                    WeightUsed = x.WeightUsed,
                    Completed = x.Completed
                }).ToList()
        };

        await _appDbContext.WorkoutLogs.AddAsync(workOutLog, cancellationToken);
        await _appDbContext.SaveChangesAsync(cancellationToken);
        
        await transaction.CreateSavepointAsync("AfterLogAndExercisesInsert", cancellationToken);
        
        // Redis Cache
        // Remove, If the user key placed from ViewProgressUserStats
        await _redisServices.RemoveRedisCacheAsync($"user_statistic:{request.UserId}", cancellationToken);
        
        // Remove, If the user key placed from ViewUserProgress
        await _redisServices.RemoveRedisCacheAsync($"user_progress:{request.UserId}", cancellationToken);
        
        // Remove, If the user key placed from ViewProgressDashboard
        await _redisServices.RemoveRedisCacheAsync("progress_dashboard", cancellationToken);
        
        return workOutLog.Id;
    }
}