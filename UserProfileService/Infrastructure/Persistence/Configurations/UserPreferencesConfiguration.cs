using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserProfileService.Domain.Entities;

namespace UserProfileService.Infrastructure.Persistence.Configurations
{
    public class UserPreferencesConfiguration : IEntityTypeConfiguration<UserPreferences>
    {
        public void Configure(EntityTypeBuilder<UserPreferences> builder)
        {
            builder.ToTable("UserPreferences");

            builder.HasKey(x => x.UserId);

            builder.Property(x => x.UserId)
                .ValueGeneratedNever();

            builder.Property(x => x.Language)
                .HasColumnType("varchar(10)")
                .HasMaxLength(10)
                .HasDefaultValue("en");

            builder.Property(x => x.Theme)
                .HasColumnType("varchar(15)")
                .HasMaxLength(15)
                .HasDefaultValue("light");

            builder.Property(x => x.WeightUnit)
                .HasColumnType("varchar(5)")
                .HasMaxLength(5)
                .HasDefaultValue("kg");

            builder.Property(x => x.HeightUnit)
                .HasColumnType("varchar(5)")
                .HasMaxLength(5)
                .HasDefaultValue("cm");

            builder.Property(x => x.DistanceUnit)
                .HasColumnType("varchar(5)")
                .HasMaxLength(5)
                .HasDefaultValue("km");
        }
    }
}
