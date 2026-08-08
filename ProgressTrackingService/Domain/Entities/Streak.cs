using System.ComponentModel.DataAnnotations.Schema;

namespace ProgressTrackingService.Domain.Entities;

public class Streak : BaseEntity<int>
{
    public int UserId { get; set; }
    
    public int CurrentStreak { get; set; }

    public int LongestStreak { get; set; }
    
    public DateTimeOffset? LastWorkoutDate { get; set; }
}