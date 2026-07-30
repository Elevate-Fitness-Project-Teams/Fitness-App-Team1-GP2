namespace NutritionService.Domain.Events;

public record CalorieTargetCalculatedEvent(
    Guid UserId,
    int DailyGoalCalories,
    decimal MinProteinGrams,
    DateTime CalculatedAtUtc);
