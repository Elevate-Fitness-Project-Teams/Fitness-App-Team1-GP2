namespace WorkoutService.Feature.WorkoutPlans.BrowseWorkoutPlans
{
    public class BrowseWorkoutPlansResponse
    {
        public string PlanId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Goal { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
    }
}