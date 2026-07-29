namespace NutritionService.Infrastructure.Messaging.Events;

/// <summary>
/// Published (fire-and-forget) every time recommendations are successfully generated
/// for a user, so other services (e.g. Progress/Analytics) can react asynchronously.
/// </summary>
public sealed record NutritionRecommendationsGeneratedEvent(
    Guid UserId,
    int RecommendedMealCount,
    int UserDailyGoalCalories,
    DateTime GeneratedAtUtc);
