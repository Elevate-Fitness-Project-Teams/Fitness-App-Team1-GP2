using Microsoft.EntityFrameworkCore;
using ProgressTrackingService.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProgressTrackingService.Infrastructure.Persistence.Configuration;

public class WorkoutLogConfiguration : IEntityTypeConfiguration<WorkoutLog>
{
    public void Configure(EntityTypeBuilder<WorkoutLog> builder)
    {
        builder.ToTable("WorkoutLogs", "ProgressTracking");
        builder.HasKey(w => w.Id);
        builder.Property(w => w.DurationInMinutes).IsRequired();
        builder.Property(w => w.CaloriesBurned).IsRequired().HasDefaultValue(0);
        builder.Property(w=> w.Rating).IsRequired().HasConversion<string>();
        builder.Property(w => w.Notes).HasMaxLength(1000);
        builder.Property(w => w.CompletedAt).IsRequired().HasColumnType("datetimeoffset");
        builder.Property(w=> w.DifficultyLevel).IsRequired().HasConversion<string>();

        builder.HasIndex(w => w.UserId);
        builder.HasIndex(w => w.SessionId);
        builder.HasIndex(x => x.CompletedAt);
        builder.HasIndex(x => new { x.UserId, x.CompletedAt });

        builder.HasMany(w => w.WorkoutLogExercises)
            .WithOne(w => w.WorkoutLog)
            .HasForeignKey(w => w.WorkoutLogId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}