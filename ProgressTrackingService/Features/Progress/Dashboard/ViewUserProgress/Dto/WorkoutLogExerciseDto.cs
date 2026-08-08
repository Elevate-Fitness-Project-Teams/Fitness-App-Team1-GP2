namespace ProgressTrackingService.Features.Progress.Dashboard.ViewUserProgress.Dto;

public class WorkoutLogExerciseDto
{
    public int Sets { get; set; }
    
    public int Reps { get; set; }

    public decimal WeightUsed { get; set; } = 0;
    
    public bool Completed { get; set; }
    
    public int WorkoutLogId { get; set; }
    
    public int ExerciseId { get; set; }
}