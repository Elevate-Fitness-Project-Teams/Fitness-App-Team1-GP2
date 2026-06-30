using WorkoutService.Domain.Enums;

namespace WorkoutService.Domain.Entities
{
    public class WorkoutPlan
    {
        public string PlanId { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public WorkoutPlanGoal Goal { get; set; }

        public WorkoutPlanStatus Status { get; set; } // depending on calories 

        public WorkoutDifficulty Difficulty { get; set; }

        public ICollection<Workout> Workouts { get; set; } = new List<Workout>();
    }
}
