using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyTrainer.Models;


namespace MyTrainer.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Activity_Exercise> ActivityExercise { get; set; }
        public DbSet<Activity_history> ActivityHistory { get; set; }
        public DbSet<Exercise> Exercise { get; set; }
        public DbSet<Weight_history> WeightHistory { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Weight_history>().
                HasOne(w => w.User)
                .WithMany(u => u.weightHistory)
                .HasForeignKey(w => w.userId)
                .HasPrincipalKey(u => u.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Activity_history>().
                HasOne(h => h.User)
                .WithMany(u => u.activityHistory)
                .HasForeignKey(h => h.userId)
                .HasPrincipalKey(u => u.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Activity_Exercise>().
                HasOne(e => e.activity_History)
                .WithMany(h => h.activityExercise)
                .HasForeignKey(e => e.activityId)
                .OnDelete(DeleteBehavior.Cascade);

            //warning
            builder.Entity<Activity_Exercise>().
                HasOne(e => e.Exercise)
                .WithMany(ex => ex.activity_Exercises)
                .HasForeignKey(e => e.exerciseId) 
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
