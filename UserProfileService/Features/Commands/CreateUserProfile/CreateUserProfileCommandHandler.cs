using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using UserProfileService.Domain.Contracts;
using UserProfileService.Domain.Entities;

namespace UserProfileService.Features.Commands.CreateUserProfile
{
    public class CreateUserProfileCommandHandler : IRequestHandler<CreateUserProfileCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateUserProfileCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(CreateUserProfileCommand request, CancellationToken cancellationToken)
        {
            var userProfile = new UserProfile
            {
                UserId = request.UserId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                MemberSince = DateTime.UtcNow,
                IsPremiumCached = false,
                Preferences = new UserPreferences
                {
                    Theme = "Light",
                    Language = "en",
                    WeightUnit = "kg",
                    HeightUnit = "cm"
                },
                NotificationSettings = new NotificationSettings
                {
                    EmailNotifications = true,
                    PushNotifications = true,
                    WeeklyReports = true
                },
                PrivacySettings = new PrivacySettings
                {
                    ProfileVisibility = "Public",
                    ShowProgressToFriends = true
                },
                Statistics = new UserStatistics
                {
                    TotalWorkouts = 0,
                    CurrentStreak = 0
                }
            };

            await _unitOfWork.UserProfiles.AddAsync(userProfile, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return userProfile.UserId;
        }
    }
}
