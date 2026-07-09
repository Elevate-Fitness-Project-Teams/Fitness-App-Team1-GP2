namespace WorkoutService.Feature.Workouts.StartWorkoutSessions
{
    public class StartWorkoutSessionRequest
    {
        public string Difficulty { get; set; } = string.Empty;
        public int PlannedDuration { get; set; }
    }
}
