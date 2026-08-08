using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserProfileService.Domain.Entities;

namespace UserProfileService.Infrastructure.Persistence.Configurations
{
    public class NotificationSettingsConfiguration : IEntityTypeConfiguration<NotificationSettings>
    {
        public void Configure(EntityTypeBuilder<NotificationSettings> builder)
        {
            builder.ToTable("NotificationSettings");

            builder.HasKey(x => x.UserId);

            builder.Property(x => x.UserId)
                .ValueGeneratedNever();

            builder.Property(x => x.WorkoutReminders)
                .HasDefaultValue(true);

            builder.Property(x => x.MealReminders)
                .HasDefaultValue(true);

            builder.Property(x => x.AchievementAlerts)
                .HasDefaultValue(true);

            builder.Property(x => x.WeeklyReports)
                .HasDefaultValue(true);

            builder.Property(x => x.EmailNotifications)
                .HasDefaultValue(true);

            builder.Property(x => x.PushNotifications)
                .HasDefaultValue(true);
        }
    }
}
