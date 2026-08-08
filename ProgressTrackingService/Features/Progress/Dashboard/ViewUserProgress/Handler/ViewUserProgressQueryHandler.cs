using MediatR;
using ProgressTrackingService.Common.Redis;
using ProgressTrackingService.Common.Response;
using ProgressTrackingService.Features.Progress.Dashboard.Shared.Dto;
using ProgressTrackingService.Features.Progress.Dashboard.ViewUserProgress.Dto;
using ProgressTrackingService.Features.Progress.Dashboard.ViewUserProgress.Query;
using ProgressTrackingService.Infrastructure.Persistence.Context;

namespace ProgressTrackingService.Features.Progress.Dashboard.ViewUserProgress.Handler;

public class ViewUserProgressQueryHandler (
    AppDbContext dbContext,
    RedisServices<ViewWeightHistoryDto> redisServices): IRequestHandler<ViewUserProgressQuery, ResponseResult<ViewWeightHistoryDto>>
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly RedisServices<ViewWeightHistoryDto> _redisServices = redisServices;

    public async Task<ResponseResult<ViewWeightHistoryDto>> Handle(ViewUserProgressQuery request, CancellationToken cancellationToken)
    {
        // Redis Cache
        var cacheKey = $"user_Achievements:{request.UserId}";
        
        var cacheResult = await _redisServices.GetRedisCacheAsync(cacheKey, cancellationToken);
        if (cacheResult.IsSuccess) return ResponseResult<ViewWeightHistoryDto>.Success(cacheResult.Data, "", StatusCode.Success);
        
        // Get All WorkoutLogs
        var allWorkoutLogs = _dbContext.WorkoutLogs
            .Where(w => w.UserId == request.UserId)
            .OrderByDescending(w => w.CompletedAt)
            .Select(w => new WorkoutLogsDto
            {
                UserId = w.UserId,
                DurationInMinutes = w.DurationInMinutes,
                CaloriesBurned = w.CaloriesBurned,
                Rating = w.Rating,
                Notes = w.Notes,
                CompletedAt = w.CompletedAt,
                DifficultyLevel = w.DifficultyLevel,
                WorkoutId = w.WorkoutId,
                SessionId = w.SessionId,
                WorkoutLogExercises = w.WorkoutLogExercises.Select(e => new WorkoutLogExerciseDto
                {
                    ExerciseId = e.ExerciseId,
                    Sets = e.SetsCompleted,
                    Reps = e.RepsCompleted,
                    WeightUsed = e.WeightUsed,
                    Completed = e.Completed,
                    WorkoutLogId = e.WorkoutLogId,
                }).ToList(),
            });
            // .ToListAsync(cancellationToken);
            // .Include(workoutLog => workoutLog.WorkoutLogExercises)

        if (!allWorkoutLogs.Any())
            return ResponseResult<ViewWeightHistoryDto>.Failure(StatusCode.NotFound, "No workout logs found.");
        
        // Get All WeightHistory
        var allWeightHistory = _dbContext.WeightHistories
            .Where(w => w.UserId == request.UserId)
            .OrderByDescending(w => w.Date);
            // .ToListAsync(cancellationToken);
        
        if (!allWeightHistory.Any())
            return ResponseResult<ViewWeightHistoryDto>.Failure(StatusCode.NotFound, "No weight history found.");
        
        // Get All UserStatistics
        var allUserStatistics = _dbContext.UserStatistics
            .Where(u => u.UserId == request.UserId)
            .OrderByDescending(w => w.UpdatedAt);
            // .ToListAsync(cancellationToken);
        
        if (!allUserStatistics.Any())
            return ResponseResult<ViewWeightHistoryDto>.Failure(StatusCode.NotFound, "No user statistics found.");

        var viewWeightHistoryDto = new ViewWeightHistoryDto
        {
            WorkoutLogs = allWorkoutLogs.Select(w => new WorkoutLogsDto
            {
                UserId = w.UserId,
                DurationInMinutes = w.DurationInMinutes,
                CaloriesBurned = w.CaloriesBurned,
                Rating = w.Rating,
                Notes = w.Notes,
                CompletedAt = w.CompletedAt,
                DifficultyLevel = w.DifficultyLevel,
                WorkoutId = w.WorkoutId,
                SessionId = w.SessionId,
                WorkoutLogExercises = w.WorkoutLogExercises.Select(e => new WorkoutLogExerciseDto
                {
                    ExerciseId = e.ExerciseId,
                    Sets = e.Sets,
                    Reps = e.Reps,
                    WeightUsed = e.WeightUsed,
                    Completed = e.Completed,
                    WorkoutLogId = e.WorkoutLogId,
                }).ToList(),
            }).ToList(),
            WeightHistory = allWeightHistory.Select(w => new WeightHistoryDto
            {
                UserId = w.UserId,
                Weight = w.Weight,
                Date = w.Date,
                Notes = w.Notes
            }).ToList(),
            UserStatistics = allUserStatistics.Select(u => new UserStatisticsDto
            {
                UserId = u.UserId,
                TotalWeightLost = u.TotalWeightLost,
                UpdatedAt = u.UpdatedAt
            }).ToList()
        };
        
        // Redis Cache
        // Make sure to remove the key, If insert new records in all steps (Workout Logs - Weight Histories - User Statistics)
        await _redisServices.SetRedisCacheAsync(cacheKey, viewWeightHistoryDto, 1, 5, cancellationToken);
        
        return ResponseResult<ViewWeightHistoryDto>.Success(viewWeightHistoryDto, "User progress history retrieved successfully.", StatusCode.Success);
    }
}