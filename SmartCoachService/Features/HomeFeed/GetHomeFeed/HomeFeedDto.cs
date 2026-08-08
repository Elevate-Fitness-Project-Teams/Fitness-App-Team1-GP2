namespace SmartCoachService.Features.HomeFeed.GetHomeFeed;

public sealed record HomeFeedDto
{
    public string DisplayName { get; init; } = default!;
    public string? AvatarUrl { get; init; }
    public bool CalorieTargetCalculated { get; init; }
    public int? DailyGoalCalories { get; init; }
    public string? TodayWorkoutName { get; init; }
    public int WorkoutsCompletedThisWeek { get; init; }
    public IReadOnlyList<string> RecommendedMealNames { get; init; } = Array.Empty<string>();
    public decimal WeightDeltaKg { get; init; }
    public int StreakDays { get; init; }
    public bool FromCache { get; init; }
}
