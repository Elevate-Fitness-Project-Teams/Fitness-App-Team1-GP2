using Microsoft.EntityFrameworkCore;
using WorkoutService.Domain.Entities;

namespace WorkoutService.Database
{
    public class WorkoutDbContext : DbContext
    {
        public WorkoutDbContext(DbContextOptions<WorkoutDbContext> options)
            : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(WorkoutDbContext).Assembly);
        }
        public DbSet<WorkoutPlan> WorkoutPlans { get; set; }
        public DbSet<Workout> Workouts { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<WorkoutExercise> WorkoutExercises { get; set; }
        public DbSet<WorkoutSession> WorkoutSessions { get; set; }
    }
}
