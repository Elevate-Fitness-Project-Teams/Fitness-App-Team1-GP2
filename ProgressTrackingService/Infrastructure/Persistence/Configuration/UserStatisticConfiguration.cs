using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProgressTrackingService.Domain.Entities;

namespace ProgressTrackingService.Infrastructure.Persistence.Configuration;

public class UserStatisticConfiguration : IEntityTypeConfiguration<UserStatistic>
{
    public void Configure(EntityTypeBuilder<UserStatistic> builder)
    {
        builder.ToTable("UserStatistics", "ProgressTracking");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.TotalWorkouts).HasDefaultValue(0).IsRequired();
        builder.Property(x => x.TotalCaloriesBurned).HasDefaultValue(0).IsRequired();
        builder.Property(x => x.TotalWeightLost).HasPrecision(18, 2).HasDefaultValue(0).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        
        builder.HasIndex(x => x.UserId);
    }
}