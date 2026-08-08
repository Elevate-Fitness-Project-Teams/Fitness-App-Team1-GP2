using MediatR;
using Microsoft.EntityFrameworkCore;
using ProgressTrackingService.Common.Redis;
using ProgressTrackingService.Common.Response;
using ProgressTrackingService.Features.Progress.Dashboard.Shared.Dto;
using ProgressTrackingService.Features.Progress.Dashboard.ViewProgressDashboard.Dto;
using ProgressTrackingService.Features.Progress.Dashboard.ViewProgressDashboard.Query;
using ProgressTrackingService.Features.Progress.Dashboard.ViewUserProgress.Dto;
using ProgressTrackingService.Infrastructure.Persistence.Context;

namespace ProgressTrackingService.Features.Progress.Dashboard.ViewProgressDashboard.Handler;

public class ViewProgressDashboardQueryHandler (
    AppDbContext appDbContext,
    RedisServices<ViewProgressDashboardDto> redisServices): IRequestHandler<ViewProgressDashboardQuery, ResponseResult<ViewProgressDashboardDto>>
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly RedisServices<ViewProgressDashboardDto> _redisServices = redisServices;
    
    public async Task<ResponseResult<ViewProgressDashboardDto>> Handle(ViewProgressDashboardQuery request, CancellationToken cancellationToken)
    {
        var cacheResult = await _redisServices.GetRedisCacheAsync("progress_dashboard", cancellationToken);
        if (cacheResult.IsSuccess) return ResponseResult<ViewProgressDashboardDto>.Success(cacheResult.Data, "", StatusCode.Success);
        
        // Step 1: weight history --> WeightHistories
        var weightHistories = await _appDbContext.WeightHistories
            .OrderByDescending(x => x.Date)
            .Select(w => new WeightHistoryDto()
            {
                UserId = w.UserId,
                Weight = w.Weight,
                Date = w.Date,
                Notes = w.Notes
            })
            .ToListAsync(cancellationToken);

        if (weightHistories.Count == 0)
            return ResponseResult<ViewProgressDashboardDto>.Failure(StatusCode.NotFound, "No weight history found.");

        // Step 2: workout history --> WorkoutLog
        var workoutLogHistory = await _appDbContext.WorkoutLogs
            .OrderByDescending(w => w.CompletedAt)
            .Select(w => new WorkoutLogsDto()
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
            })
            .ToListAsync(cancellationToken);

        if (workoutLogHistory.Count == 0)
            return ResponseResult<ViewProgressDashboardDto>.Failure(StatusCode.NotFound, "No workout log history found.");
        
        // Step 3: achievements --> Achievements
        var achievements = await _appDbContext.Achievements
            .Select(a => new DashboardUserAchievementDto()
            {
                Name = a.Name,
                Description = a.Description,
                IconUrl = a.IconUrl
            })
            .ToListAsync(cancellationToken);

        if (achievements.Count == 0) return ResponseResult<ViewProgressDashboardDto>.Failure(StatusCode.NotFound, "No workout log history found.");

        var viewProgressDashboardDto = new ViewProgressDashboardDto()
        {
            WeightHistory = weightHistories,
            WorkoutLogsHistory = workoutLogHistory,
            UserAchievements = achievements,
        };
        
        // Redis Cache
        // Make sure to remove the key, If insert new records in all steps (Weight Histories - Workout Logs - Achievements)
        await _redisServices.SetRedisCacheAsync("progress_dashboard", viewProgressDashboardDto, 1, 5, cancellationToken);
        
        return ResponseResult<ViewProgressDashboardDto>.Success(viewProgressDashboardDto, "Progress history retrieved successfully.", StatusCode.Success);
    }
}