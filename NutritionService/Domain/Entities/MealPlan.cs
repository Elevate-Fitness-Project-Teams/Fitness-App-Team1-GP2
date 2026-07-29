using NutritionService.Common.Abstractions;

namespace NutritionService.Domain.Entities;

public sealed class MealPlan : BaseEntity
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public int TargetCalorieRangeMin { get; set; }
    public int TargetCalorieRangeMax { get; set; }

    public ICollection<MealPlanItem> Items { get; set; } = new List<MealPlanItem>();
}
