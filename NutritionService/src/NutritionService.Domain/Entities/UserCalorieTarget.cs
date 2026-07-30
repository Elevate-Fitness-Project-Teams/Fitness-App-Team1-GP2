namespace NutritionService.Domain.Entities;

public class UserCalorieTarget
{
    public Guid UserId { get; set; }
    public int DailyGoalCalories { get; set; }
    public decimal MinProteinGrams { get; set; }
    public DateTime CalculatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
