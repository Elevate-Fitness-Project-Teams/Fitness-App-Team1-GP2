namespace SmartCoachService.Application.Features.GetHomeFeed.Dtos;

public sealed record HomeFeedDto(
    object ProfileSummary,
    object FceMetrics,
    object WorkoutSuggestions,
    object MealRecommendations,
    object ProgressSnapshot,
    bool FromCache,
    DateTime GeneratedAtUtc);
