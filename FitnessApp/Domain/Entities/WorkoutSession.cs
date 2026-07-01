using WorkoutService.Domain.Enums;

namespace WorkoutService.Domain.Entities
{
    public class WorkoutSession 
    {
        public string SessionId { get; set; } = string.Empty;

        public int UserId { get; set; }

        public int WorkoutId { get; set; }

        public DateTime StartedAt { get; set; }

        public WorkoutSessionStatus Status { get; set; }

        public Workout? Workout { get; set; }
    }
}
