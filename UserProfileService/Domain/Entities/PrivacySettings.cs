namespace UserProfileService.Domain.Entities
{
    public class PrivacySettings
    {
        public int UserId { get; set; }
        public string ProfileVisibility { get; set; } = "private";
        public bool ShowProgressToFriends { get; set; }
        public bool AllowDataSharing { get; set; }

        // Navigation Property
        public UserProfile? UserProfile { get; set; }
    }
}
