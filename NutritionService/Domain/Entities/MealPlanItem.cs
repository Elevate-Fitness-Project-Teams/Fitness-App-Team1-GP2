using NutritionService.Common.Abstractions;
using NutritionService.Domain.Enums;

namespace NutritionService.Domain.Entities;

/// <summary>A single scheduled meal slot inside a MealPlan (e.g. Day 3 / Lunch -> Meal X).</summary>
public sealed class MealPlanItem : BaseEntity
{
    public Guid MealPlanId { get; set; }
    public MealPlan MealPlan { get; set; } = default!;

    public Guid MealId { get; set; }
    public Meal Meal { get; set; } = default!;

    public int DayNumber { get; set; }
    public MealType MealType { get; set; }
    public int OrderIndex { get; set; }
}
