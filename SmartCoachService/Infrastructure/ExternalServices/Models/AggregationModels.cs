namespace SmartCoachService.Infrastructure.ExternalServices.Models;

public sealed class ProfileSummary
{
    public Guid UserId { get; set; }
    public string DisplayName { get; set; } = default!;
    public string? AvatarUrl { get; set; }
}

public sealed class FceSummary
{
    public Guid UserId { get; set; }
    public bool IsCalculated { get; set; }
    public int DailyGoalCalories { get; set; }
}

public sealed class WorkoutSummary
{
    public Guid UserId { get; set; }
    public string? TodayWorkoutName { get; set; }
    public int CompletedThisWeek { get; set; }
}

public sealed class NutritionSummary
{
    public Guid UserId { get; set; }
    public IReadOnlyList<string> RecommendedMealNames { get; set; } = Array.Empty<string>();
}

public sealed class ProgressSummary
{
    public Guid UserId { get; set; }
    public decimal WeightDeltaKg { get; set; }
    public int StreakDays { get; set; }
}

/// <summary>Aggregate handed back from the Progress Service, reused as chat-prompt context in Epic 7.1.</summary>
public sealed class UserContextBundle
{
    public FceSummary? Fce { get; set; }
    public ProgressSummary? Progress { get; set; }
}
