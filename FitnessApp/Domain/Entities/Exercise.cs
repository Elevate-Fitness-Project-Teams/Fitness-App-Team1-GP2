using WorkoutService.Domain.Enums;

namespace WorkoutService.Domain.Entities
{
    public class Exercise
    {
        public int ExerciseId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string TargetMuscles { get; set; } = string.Empty;

        public string Equipment { get; set; } = string.Empty;

        public WorkoutDifficulty Difficulty { get; set; }

        public string Description { get; set; } = string.Empty;

        public string? VideoUrl { get; set; }

        public ICollection<WorkoutExercise> WorkoutExercises { get; set; } = new List<WorkoutExercise>();
    }
}
