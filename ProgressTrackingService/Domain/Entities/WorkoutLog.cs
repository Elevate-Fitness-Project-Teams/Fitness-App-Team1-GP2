using ProgressTrackingService.Domain.Enums;

namespace ProgressTrackingService.Domain.Entities;

/// <summary>
///     
/// </summary>
public class WorkoutLog : BaseEntity<int>
{
    public int DurationInMinutes { get; set; }
    
    public int CaloriesBurned { get; set; }

    public Rating Rating { get; set; }
    
    public string? Notes { get; set; } = string.Empty;
    
    public DateTimeOffset CompletedAt { get; set; }
    
    public DifficultyLevel DifficultyLevel { get; set; }

    #region Relationship

    // FK
    public int UserId { get; set; }
    
    // FK
    public int WorkoutId { get; set; }
    
    public List<WorkoutLogExercise> WorkoutLogExercises { get; set; } = new List<WorkoutLogExercise>();
    
    // FK
    public string SessionId { get; set; } = string.Empty;

    #endregion
}