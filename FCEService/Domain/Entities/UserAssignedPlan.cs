using FCEService.Domain.Enums;

namespace FCEService.Domain.Entities
{
    public class UserAssignedPlan : BaseEntity
    {
        public Guid userId { get; set; }
        public Goal goal { get; private set; }
        public double calorieIntake { get; private set; } //CalroieAllocation
        public string? WorkoutPlan { get; private set; }
        public string? NutritionPlan { get; private set; }
        public bool IsActive { get; set; } = true;

        private UserAssignedPlan() { }

        public static UserAssignedPlan Create(Guid userId, Goal goal, double calorieIntake, string? workoutPlan, string? nutritionPlan)
            => new UserAssignedPlan
            {
                userId = userId,
                goal = goal,
                calorieIntake = calorieIntake,
                WorkoutPlan = workoutPlan,
                NutritionPlan = nutritionPlan,
                IsActive = true
            };
    }
}
