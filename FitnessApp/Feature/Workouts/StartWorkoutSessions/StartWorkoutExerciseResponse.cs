namespace WorkoutService.Feature.Workouts.StartWorkoutSessions
{
    public sealed class StartWorkoutExerciseResponse
    {
        public int ExerciseId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int SetsDefault { get; set; }
        public int RepsDefault { get; set; }
        public int RestTimeInSeconds { get; set; }
        public int OrderIndex { get; set; }
    }
}