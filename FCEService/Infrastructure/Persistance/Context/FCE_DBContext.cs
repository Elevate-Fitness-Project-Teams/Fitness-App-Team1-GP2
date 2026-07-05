using Microsoft.EntityFrameworkCore;
using FCEService.Infrastructure.Persistance.Configuration;

namespace FCEService.Infrastructure.Persistance.Context
{
    public class FCE_DBContext : DbContext
    {
        public FCE_DBContext(DbContextOptions<FCE_DBContext> options) : base(options)
        {
        }
        public DbSet<Domain.Entities.UserPlanHistory> UserPlanHistories { get; set; }
        public DbSet<Domain.Entities.UserAssignedPlan> UserAssignedPlans { get; set; }
        public DbSet<Domain.Aggregates.CalculatedMetrics> CalculatedMetrics { get; set; }
        public DbSet<Domain.Aggregates.UserFitnessStats> UserFitnessStats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new CalculatedMetricsConfiguration());
            modelBuilder.ApplyConfiguration(new UserAssignedPlansConfiguration());
            modelBuilder.ApplyConfiguration(new UserFitnessStatsConfiguration());
            modelBuilder.ApplyConfiguration(new UserPlanHistoryConfiguration());
        }
    }
}
