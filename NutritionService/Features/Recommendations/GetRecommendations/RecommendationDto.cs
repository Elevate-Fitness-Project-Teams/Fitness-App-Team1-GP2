using NutritionService.Domain.Enums;

namespace NutritionService.Features.Recommendations.GetRecommendations;

public sealed class MealRecommendationDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public MealType Type { get; init; }
    public int Calories { get; init; }
    public decimal Protein { get; init; }
    public decimal Carbs { get; init; }
    public decimal Fat { get; init; }
    public List<string> Tags { get; init; } = new();
}

public sealed class GetRecommendationsResult
{
    public int UserDailyGoalCalories { get; init; }
    public IReadOnlyList<MealRecommendationDto> RecommendedMeals { get; init; } = Array.Empty<MealRecommendationDto>();
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
}
