using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProgressTrackingService.Domain.Entities;

namespace ProgressTrackingService.Infrastructure.Persistence.Configuration;

public class BodyMeasurementConfiguration : IEntityTypeConfiguration<BodyMeasurement>
{
    public void Configure(EntityTypeBuilder<BodyMeasurement> builder)
    {
        builder.ToTable("BodyMeasurements", "ProgressTracking");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Neck).HasPrecision(5, 2).IsRequired(false);
        builder.Property(b => b.Chest).HasPrecision(5, 2).IsRequired(false);
        builder.Property(b => b.Biceps).HasPrecision(5, 2).IsRequired(false);
        builder.Property(b => b.Waist).HasPrecision(5, 2).IsRequired(false);
        builder.Property(b => b.Hips).HasPrecision(5, 2).IsRequired(false);
        builder.Property(b => b.Thighs).HasPrecision(5, 2).IsRequired(false);
        builder.Property(b => b.RecordedAt).IsRequired().HasColumnType("datetimeoffset");
        
        builder.HasIndex(b => b.UserId);
    }
}