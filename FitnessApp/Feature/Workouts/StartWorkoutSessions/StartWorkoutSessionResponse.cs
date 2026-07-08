namespace WorkoutService.Feature.Workouts.StartWorkoutSessions
{
    public sealed class StartWorkoutSessionResponse
    {
        public string SessionId { get; set; } = string.Empty;
        public bool IsNewSession { get; set; }
        public int WorkoutId { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime StartedAt { get; set; }

        public List<StartWorkoutExerciseResponse> Exercises { get; set; } = [];
    }
}