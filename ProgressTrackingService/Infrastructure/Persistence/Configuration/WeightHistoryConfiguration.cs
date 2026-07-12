using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProgressTrackingService.Domain.Entities;

namespace ProgressTrackingService.Infrastructure.Persistence.Configuration;

public class WeightHistoryConfiguration : IEntityTypeConfiguration<WeightHistory>
{
    [Obsolete("Obsolete")]
    public void Configure(EntityTypeBuilder<WeightHistory> builder)
    {
        builder.ToTable("WeightHistories", "ProgressTracking");
        builder.HasKey(w => w.Id);
        builder.Property(w => w.Weight).HasPrecision(5, 2).IsRequired();
        builder.Property(w => w.Date).IsRequired().HasColumnType("datetimeoffset");
        builder.Property(w => w.Notes).HasMaxLength(500).IsRequired(false);
        
        builder.HasIndex(w => w.UserId);
    }
}