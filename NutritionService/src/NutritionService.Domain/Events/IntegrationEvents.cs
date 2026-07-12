namespace SmartCoachService.Domain.Events;

/// <summary>Published by FCE Service on "fce.calorie-target.calculated" exchange.</summary>
public record CalorieTargetCalculatedEvent(
    Guid UserId,
    int DailyGoalCalories,
    decimal MinProteinGrams,
    string? FitnessGoal,
    DateTime CalculatedAtUtc);

/// <summary>Published by Progress Service on "progress.snapshot.updated" exchange.</summary>
public record ProgressUpdatedEvent(
    Guid UserId,
    decimal CurrentWeightKg,
    decimal? BodyFatPercentage,
    int CompletedWorkoutsLast30Days,
    DateTime UpdatedAtUtc);
