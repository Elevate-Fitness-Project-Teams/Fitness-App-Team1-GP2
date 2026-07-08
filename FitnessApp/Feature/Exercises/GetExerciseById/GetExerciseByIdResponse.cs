namespace WorkoutService.Feature.Exercises.GetExerciseById
{
    public class GetExerciseByIdResponse
    {
        public int ExerciseId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string TargetMuscles { get; set; } = string.Empty;
        public string Equipment { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? VideoUrl { get; set; }
    }
}