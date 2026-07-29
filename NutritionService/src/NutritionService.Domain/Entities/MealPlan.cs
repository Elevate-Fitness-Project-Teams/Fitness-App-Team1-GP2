namespace NutritionService.Domain.Entities;

public class MealPlan
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public int TargetCalorieRangeMin { get; set; }
    public int TargetCalorieRangeMax { get; set; }

    public ICollection<MealPlanItem> Items { get; set; } = new List<MealPlanItem>();
}

public class MealPlanItem
{
    public Guid Id { get; set; }
    public Guid MealPlanId { get; set; }
    public MealPlan MealPlan { get; set; } = default!;

    public Guid MealId { get; set; }
    public Meal Meal { get; set; } = default!;

    public int DayNumber { get; set; }     // e.g. 1..7
    public string MealSlot { get; set; } = default!; // Breakfast/Lunch/Dinner/Snack
}
