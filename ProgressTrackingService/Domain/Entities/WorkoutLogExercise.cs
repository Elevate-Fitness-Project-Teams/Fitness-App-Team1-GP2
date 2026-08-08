namespace ProgressTrackingService.Domain.Entities;

public class WorkoutLogExercise : BaseEntity<int>
{
    public int SetsCompleted { get; set; }
    
    public int RepsCompleted { get; set; }

    public decimal WeightUsed { get; set; } = 0;
    
    public bool Completed { get; set; }
    
    #region Relationship

    public int WorkoutLogId { get; set; }
    
    public WorkoutLog WorkoutLog { get; set; } = null!;
    
    public int ExerciseId { get; set; }

    #endregion
}