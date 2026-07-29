namespace SmartCoachService.Domain.Entities;

public class UserFceContext
{
    public Guid UserId { get; set; }
    public int DailyGoalCalories { get; set; }
    public decimal MinProteinGrams { get; set; }
    public string? FitnessGoal { get; set; }   // e.g. "WeightLoss", "MuscleGain"
    public DateTime UpdatedAtUtc { get; set; }
}

public class UserProgressContext
{
    public Guid UserId { get; set; }
    public decimal CurrentWeightKg { get; set; }
    public decimal? BodyFatPercentage { get; set; }
    public int CompletedWorkoutsLast30Days { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
