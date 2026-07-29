using NutritionService.Common.Abstractions;
using NutritionService.Domain.Enums;

namespace NutritionService.Domain.Entities;

public sealed class Meal : BaseEntity
{
    public string Name { get; set; } = default!;
    public MealType Type { get; set; }

    public int Calories { get; set; }
    public decimal Protein { get; set; }
    public decimal Carbs { get; set; }
    public decimal Fat { get; set; }

    // Stored as JSON columns (see Infrastructure/Persistence/Configurations/MealConfiguration.cs)
    public List<string> Ingredients { get; set; } = new();
    public string Instructions { get; set; } = string.Empty;
    public List<string> Variations { get; set; } = new();
    public List<string> Allergens { get; set; } = new();
    public List<string> Tags { get; set; } = new();

    public ICollection<MealPlanItem> MealPlanItems { get; set; } = new List<MealPlanItem>();
}
