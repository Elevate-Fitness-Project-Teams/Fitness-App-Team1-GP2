using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotificationService.Domain.Notifications;

namespace NotificationService.Infrastructure.Persistence.Configurations;

public sealed class InAppNotificationConfiguration
    : IEntityTypeConfiguration<InAppNotification>
{
    public void Configure(EntityTypeBuilder<InAppNotification> builder)
    {
        builder.ToTable("InAppNotifications");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(150)
            .IsUnicode(false);

        builder.Property(x => x.Message)
            .IsRequired()
            .HasMaxLength(500)
            .IsUnicode(false);

        builder.Property(x => x.Type)
            .IsRequired()
            .HasMaxLength(30)
            .IsUnicode(false)
            .HasConversion<string>();

        builder.Property(x => x.IsRead)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.SentAt)
            .IsRequired();

        builder.HasIndex(x => x.UserId);
    }
}