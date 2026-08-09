using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProgressTrackingService.Domain.Entities;

namespace ProgressTrackingService.Infrastructure.Persistence.Configuration;

public class WorkoutSessionTrackingsConfiguration : IEntityTypeConfiguration<WorkoutSessionTracking>
{
    public void Configure(EntityTypeBuilder<WorkoutSessionTracking> builder)
    {
        builder.ToTable("WorkoutSessionTrackings", "ProgressTracking");
    }
}