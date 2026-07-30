using MediatR;
using SmartCoachService.Common.Abstractions;
using SmartCoachService.Domain.Entities;
using SmartCoachService.Infrastructure.ExternalServices;
using System.Text.Json;

namespace SmartCoachService.Features.HomeFeed.GetHomeFeed;

/// <summary>
/// Cache-first edge-aggregator: serves RecommendationCache.HomeFeedDataJson directly while
/// CurrentTime &lt; ExpiresAt; otherwise aggregates Profile + FCE + Workout + Nutrition + Progress,
/// upserts the cache row, and returns the fresh payload. Any downstream failure bubbles up as
/// ServiceUnavailableException (503 SRV_SERVICE_UNAVAILABLE), handled by ClientHelpers.GetOrNullAsync callers.
/// </summary>
public sealed class GetHomeFeedQueryHandler : IRequestHandler<GetHomeFeedQuery, HomeFeedDto>
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(15);

    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IProfileServiceClient _profileClient;
    private readonly IFceServiceClient _fceClient;
    private readonly IWorkoutServiceClient _workoutClient;
    private readonly INutritionServiceClient _nutritionClient;
    private readonly IProgressServiceClient _progressClient;

    public GetHomeFeedQueryHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IProfileServiceClient profileClient,
        IFceServiceClient fceClient,
        IWorkoutServiceClient workoutClient,
        INutritionServiceClient nutritionClient,
        IProgressServiceClient progressClient)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _profileClient = profileClient;
        _fceClient = fceClient;
        _workoutClient = workoutClient;
        _nutritionClient = nutritionClient;
        _progressClient = progressClient;
    }

    public async Task<HomeFeedDto> Handle(GetHomeFeedQuery request, CancellationToken cancellationToken)
    {
        var cacheEntry = await _unitOfWork.RecommendationCaches.FirstOrDefaultAsync(
            c => c.UserId == _currentUser.UserId, cancellationToken);

        if (cacheEntry is not null && cacheEntry.ExpiresAt > DateTime.UtcNow)
        {
            var cached = JsonSerializer.Deserialize<HomeFeedDto>(cacheEntry.HomeFeedDataJson)!;
            return cached with { FromCache = true };
        }

        // Fan out — a failure in any one of these throws ServiceUnavailableException (503).
        var profile = await _profileClient.GetProfileAsync(_currentUser.UserId, cancellationToken);
        var fce = await _fceClient.GetCalorieTargetAsync(_currentUser.UserId, cancellationToken);
        var workout = await _workoutClient.GetTodaysWorkoutAsync(_currentUser.UserId, cancellationToken);
        var nutrition = await _nutritionClient.GetTopRecommendationsAsync(_currentUser.UserId, cancellationToken);
        var progress = await _progressClient.GetProgressSummaryAsync(_currentUser.UserId, cancellationToken);

        var freshFeed = new HomeFeedDto
        {
            DisplayName = profile?.DisplayName ?? "there",
            AvatarUrl = profile?.AvatarUrl,
            CalorieTargetCalculated = fce?.IsCalculated ?? false,
            DailyGoalCalories = fce?.IsCalculated == true ? fce.DailyGoalCalories : null,
            TodayWorkoutName = workout?.TodayWorkoutName,
            WorkoutsCompletedThisWeek = workout?.CompletedThisWeek ?? 0,
            RecommendedMealNames = nutrition?.RecommendedMealNames ?? Array.Empty<string>(),
            WeightDeltaKg = progress?.WeightDeltaKg ?? 0,
            StreakDays = progress?.StreakDays ?? 0,
            FromCache = false
        };

        var serialized = JsonSerializer.Serialize(freshFeed);

        if (cacheEntry is null)
        {
            cacheEntry = new RecommendationCache
            {
                UserId = _currentUser.UserId,
                HomeFeedDataJson = serialized,
                ExpiresAt = DateTime.UtcNow.Add(CacheTtl)
            };
            await _unitOfWork.RecommendationCaches.AddAsync(cacheEntry, cancellationToken);
        }
        else
        {
            cacheEntry.HomeFeedDataJson = serialized;
            cacheEntry.ExpiresAt = DateTime.UtcNow.Add(CacheTtl);
            _unitOfWork.RecommendationCaches.Update(cacheEntry);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return freshFeed;
    }
}
