namespace WorkoutService.Feature.Workouts.GetWorkoutDetails
{
    public class WorkoutExerciseResponse
    {
        public int ExerciseId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string TargetMuscles { get; set; } = string.Empty;
        public string Equipment { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? VideoUrl { get; set; }

        public int SetsDefault { get; set; }
        public int RepsDefault { get; set; }
        public int RestTimeInSeconds { get; set; }
        public int OrderIndex { get; set; }
    }
}
