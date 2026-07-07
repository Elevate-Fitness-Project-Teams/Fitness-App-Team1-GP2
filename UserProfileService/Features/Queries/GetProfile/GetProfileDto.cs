using System;

namespace FitnessApp.UserProfileService.Features.Queries.GetProfile
{
    public class GetProfileDto
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public bool IsPremiumCached { get; set; }
        public DateTime MemberSince { get; set; }
        
        // Statistics
        public int TotalWorkouts { get; set; }
        public int CurrentStreak { get; set; }
    }
}
