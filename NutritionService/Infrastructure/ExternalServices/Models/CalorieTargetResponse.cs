namespace NutritionService.Infrastructure.ExternalServices.Models;

/// <summary>Mirrors the FCE Service response for GET /api/v1/fce/calorie-target/{userId}.</summary>
public sealed class CalorieTargetResponse
{
    public Guid UserId { get; set; }
    public bool IsCalculated { get; set; }
    public int DailyGoalCalories { get; set; }
    public decimal MinProtein { get; set; }
}
