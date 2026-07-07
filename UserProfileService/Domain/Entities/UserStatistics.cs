namespace UserProfileService.Domain.Entities
{
    public class UserStatistics
    {
        public int UserId { get; set; }
        public int TotalWorkouts { get; set; }
        public int CurrentStreak { get; set; }

        // Navigation Property
        public UserProfile? UserProfile { get; set; }
    }
}
