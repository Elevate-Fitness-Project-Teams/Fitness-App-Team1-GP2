using NutritionService.Domain.Enums;

namespace NutritionService.Features.Meals.GetMealDetail;

public sealed class MealDetailDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public MealType Type { get; init; }
    public int Calories { get; init; }
    public decimal Protein { get; init; }
    public decimal Carbs { get; init; }
    public decimal Fat { get; init; }
    public List<string> Ingredients { get; init; } = new();
    public string Instructions { get; init; } = string.Empty;
    public List<string> Variations { get; init; } = new();
    public List<string> Allergens { get; init; } = new();
    public List<string> Tags { get; init; } = new();
}
