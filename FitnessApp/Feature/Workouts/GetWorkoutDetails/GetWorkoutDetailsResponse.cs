namespace WorkoutService.Feature.Workouts.GetWorkoutDetails
{
    public class GetWorkoutDetailsResponse
    {
        public int WorkoutId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public int DurationInMinutes { get; set; }
        public string PlanId { get; set; } = string.Empty;
        public string PlanName { get; set; } = string.Empty;

        public List<WorkoutExerciseResponse> Exercises { get; set; } = [];
    }
}

