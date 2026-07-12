using MediatR;
using ProgressTrackingService.Common.Pagination;
using ProgressTrackingService.Common.Redis;
using ProgressTrackingService.Common.Response;
using ProgressTrackingService.Features.Progress.Achievements.ViewAchievementsUser.Dto;
using ProgressTrackingService.Features.Progress.Achievements.ViewAchievementsUser.Query;
using ProgressTrackingService.Infrastructure.Persistence.Context;

namespace ProgressTrackingService.Features.Progress.Achievements.ViewAchievementsUser.Handler;

public class ViewAchievementsUserQueryHandler(
    AppDbContext appDbContext,
    RedisServices<PaginatedResult<UserAchievementDto>> redisServices): IRequestHandler<ViewAchievementsUserQuery, ResponseResult<PaginatedResult<UserAchievementDto>>>
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly RedisServices<PaginatedResult<UserAchievementDto>> _redisServices = redisServices;
    
    public async Task<ResponseResult<PaginatedResult<UserAchievementDto>>> Handle(ViewAchievementsUserQuery request, CancellationToken cancellationToken)
    {
        // Redis Cache
        var cacheKey = $"user_Achievements:{request.UserId}:page_Number:{request.PageNumber}:page_Size{request.PageSize}";
        
        var cacheResult = await _redisServices.GetRedisCacheAsync(cacheKey, cancellationToken);
        if (cacheResult.IsSuccess) return ResponseResult<PaginatedResult<UserAchievementDto>>.Success(cacheResult.Data,"Retrieve from cache",StatusCode.Success);
        
        var userAchievements = _appDbContext.UserAchievements
            .Where(ua => ua.UserId == request.UserId)
            .Select(ua => new UserAchievementDto()
            {
                Name = ua.Achievement!.Name,
                Description = ua.Achievement!.Description,
                IconUrl = ua.Achievement!.IconUrl,
                UserId = ua.UserId,
                EarnedAt = ua.AchievedAt
            });
        
        var pagedResult = await userAchievements.ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
        
        if (pagedResult.TotalCount == 0)
            return ResponseResult<PaginatedResult<UserAchievementDto>>.Failure(StatusCode.NotFound, "No achievements found for the user.");
        
        // Redis Cache
        // Make sure to remove the key, If insert new records in all steps (Workout Logs)
        await _redisServices.SetRedisCacheAsync(cacheKey, pagedResult, 1, 5, cancellationToken);
        
        // Remove, If the user key placed from ViewProgressDashboard
        await _redisServices.RemoveRedisCacheAsync("progress_dashboard", cancellationToken);
        
        return ResponseResult<PaginatedResult<UserAchievementDto>>.Success(pagedResult, "User achievements retrieved successfully.", StatusCode.Success);
    }
}