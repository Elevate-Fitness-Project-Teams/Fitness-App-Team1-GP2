using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NotificationService.Domain.Notifications;
using NotificationService.Infrastructure.Persistence.Configurations;
using System.Reflection;

namespace NotificationService.Infrastructure.Persistence
{
    public class NotificationDbContext : DbContext
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly((typeof(NotificationDbContext)).Assembly);
        }

        public DbSet<InAppNotification> InAppNotifications { get; set; }

    }
    }

