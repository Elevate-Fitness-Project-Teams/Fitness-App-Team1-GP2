using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProgressTrackingService.Domain.Entities;

namespace ProgressTrackingService.Infrastructure.Persistence.Configuration;

public class StreakConfiguration : IEntityTypeConfiguration<Streak>
{
    public void Configure(EntityTypeBuilder<Streak> builder)
    {
        builder.ToTable("Streaks", "ProgressTracking");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CurrentStreak).HasDefaultValue(0).IsRequired();
        builder.Property(x => x.LongestStreak).HasDefaultValue(0).IsRequired();
        builder.Property(x => x.LastWorkoutDate).HasColumnType("datetimeoffset").IsRequired(false);
        
        builder.HasIndex(x => x.UserId);
    }
}