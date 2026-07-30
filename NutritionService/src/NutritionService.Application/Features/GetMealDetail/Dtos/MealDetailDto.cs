namespace NutritionService.Application.Features.GetMealDetail.Dtos;

public sealed record MealDetailDto(
    Guid Id,
    string Name,
    string Type,
    int Calories,
    decimal ProteinGrams,
    decimal CarbsGrams,
    decimal FatGrams,
    object Ingredients,
    object Instructions,
    object Variations,
    object Allergens,
    object Tags);
