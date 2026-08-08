using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProgressTrackingService.Domain.Entities;

namespace ProgressTrackingService.Infrastructure.Persistence.Configuration;

public class AchievementConfiguration : IEntityTypeConfiguration<Achievement>
{
    public void Configure(EntityTypeBuilder<Achievement> builder)
    {
        builder.ToTable("Achievements", "ProgressTracking");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Name).HasMaxLength(100).IsRequired();
        builder.Property(a => a.Description).HasMaxLength(255).IsRequired(false);
        builder.Property(a => a.IconUrl).HasMaxLength(255).IsRequired(false);
        
        builder.HasIndex(a => a.Name);
        
        builder.HasMany(a => a.UserAchievements)
            .WithOne(a => a.Achievement)
            .HasForeignKey(a => a.AchievementId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}