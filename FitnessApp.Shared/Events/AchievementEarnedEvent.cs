namespace FitnessApp.Shared.Events
{
    public class AchievementEarnedEvent
    {
        public int UserId { get; set; }

        public string AchievementName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTimeOffset EarnedAt { get; set; }
    }
}