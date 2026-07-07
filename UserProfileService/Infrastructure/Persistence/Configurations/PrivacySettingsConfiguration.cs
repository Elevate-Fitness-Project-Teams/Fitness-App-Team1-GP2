using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserProfileService.Domain.Entities;

namespace UserProfileService.Infrastructure.Persistence.Configurations
{
    public class PrivacySettingsConfiguration : IEntityTypeConfiguration<PrivacySettings>
    {
        public void Configure(EntityTypeBuilder<PrivacySettings> builder)
        {
            builder.ToTable("PrivacySettings");

            builder.HasKey(x => x.UserId);

            builder.Property(x => x.UserId)
                .ValueGeneratedNever();

            builder.Property(x => x.ProfileVisibility)
                .HasColumnType("varchar(20)")
                .HasMaxLength(20)
                .HasDefaultValue("private");

            builder.Property(x => x.ShowProgressToFriends)
                .HasDefaultValue(false);

            builder.Property(x => x.AllowDataSharing)
                .HasDefaultValue(false);
        }
    }
}
