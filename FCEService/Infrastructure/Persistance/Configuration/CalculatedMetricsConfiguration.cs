using FCEService.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCEService.Infrastructure.Persistance.Configuration
{
    public class CalculatedMetricsConfiguration : IEntityTypeConfiguration<CalculatedMetrics>
    {
        public void Configure(EntityTypeBuilder<CalculatedMetrics> builder)
        {
            builder.ToTable("CalculatedMetrics");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.UserId).IsUnique(); // upsert key — one current snapshot per user

            builder.Property(x => x.UserId)
                   .IsRequired();

            builder.Property(x => x.BMR)
                   .HasColumnName("BMR")
                   .IsRequired();

            builder.Property(x => x.TDEE)
                   .HasColumnName("TDEE")
                   .IsRequired();

            builder.Property(x => x.CalorieTarget)
                   .HasColumnName("CalorieTarget")
                   .IsRequired(); 

            builder.Property(x => x.BMRStatus)
                   .HasColumnName("BMRStatus")
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            builder.OwnsOne(x => x.BMRRange, navigation =>
            {
                navigation.Property(b => b.Min)
                          .HasColumnName("BMRRangeMin");

                navigation.Property(b => b.Max)
                          .HasColumnName("BMRRangeMax");
            });
        }
    }
}
