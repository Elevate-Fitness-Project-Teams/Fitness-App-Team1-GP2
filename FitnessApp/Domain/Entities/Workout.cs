using WorkoutService.Domain.Enums;

namespace WorkoutService.Domain.Entities
{
    public class Workout 
    {
        public int WorkoutId { get; set; }

        public string PlanId { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public WorkoutCategory Category { get; set; }

        public int DurationInMinutes { get; set; }

        public WorkoutDifficulty Difficulty { get; set; }

        public WorkoutPlan? WorkoutPlan { get; set; }

        public ICollection<WorkoutExercise> WorkoutExercises { get; set; } = new List<WorkoutExercise>();

        public ICollection<WorkoutSession> WorkoutSessions { get; set; } = new List<WorkoutSession>();
    }
}
