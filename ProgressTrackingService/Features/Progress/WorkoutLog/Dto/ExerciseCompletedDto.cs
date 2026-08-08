namespace ProgressTrackingService.Features.Progress.WorkoutLog.Dto;

public class ExerciseCompletedDto
{
    public int Sets { get; set; }

    public int Reps { get; set; }

    public decimal WeightUsed { get; set; } = 0;
    
    public bool Completed { get; set; }
    
    public int ExerciseId { get; set; }
}