namespace NutritionService.Domain.Entities;

public class Meal
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Type { get; set; } = default!; // Breakfast, Lunch, Dinner, Snack
    public int Calories { get; set; }
    public decimal ProteinGrams { get; set; }
    public decimal CarbsGrams { get; set; }
    public decimal FatGrams { get; set; }

    public string IngredientsJson { get; set; } = "[]";
    public string InstructionsJson { get; set; } = "[]";
    public string VariationsJson { get; set; } = "[]";
    public string AllergensJson { get; set; } = "[]";
    public string TagsJson { get; set; } = "[]";
}
