namespace SmartCoachService.Application.Common.Interfaces;

public record HomeFeedPayload(
    object ProfileSummary,
    object FceMetrics,
    object WorkoutSuggestions,
    object MealRecommendations,
    object ProgressSnapshot);

public interface IHomeFeedAggregator
{
    Task<HomeFeedPayload> AggregateAsync(Guid userId, CancellationToken ct = default);
}
