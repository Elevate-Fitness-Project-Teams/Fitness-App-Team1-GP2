using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProgressTrackingService.Domain.Entities;

namespace ProgressTrackingService.Infrastructure.Persistence.Configuration;

public class UserAchievementConfiguration : IEntityTypeConfiguration<UserAchievement>
{
    public void Configure(EntityTypeBuilder<UserAchievement> builder)
    {
        builder.ToTable("UserAchievements", "ProgressTracking");
        builder.HasKey(ua => ua.Id);
        builder.Property(ua => ua.AchievedAt).HasColumnType("datetimeoffset").IsRequired();
        
        builder.HasIndex(ua => ua.UserId);
    }
}