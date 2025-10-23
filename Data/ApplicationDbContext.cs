using Microsoft.EntityFrameworkCore;
using TimeTrackerAPI.Models;

namespace TimeTrackerAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        { }

        public DbSet<Project> Projects { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ProjectTime> ProjectTimes { get; set; }
        public DbSet<UserIdentityProvider> UserIdentityProviders { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Project (1) -> (many) ProjectTimes with cascade delete
            modelBuilder.Entity<ProjectTime>()
                .HasOne(pt => pt.Project)
                .WithMany(p => p.ProjectTimes)
                .HasForeignKey(pt => pt.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProjectTime>()
                .HasIndex(pt => new { pt.UserId, pt.ProjectId, pt.EndTime });

            // Index for “latest entry per project” queries
            modelBuilder.Entity<ProjectTime>()
                .HasIndex(pt => new { pt.UserId, pt.ProjectId, pt.StartTime });

            modelBuilder.Entity<UserIdentityProvider>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasIndex(x => new { x.Provider, x.ProviderUserId }).IsUnique();
                e.HasOne(x => x.User)
                     .WithMany(u => u.ExternalLogins)
                     .HasForeignKey(x => x.UserId)
                     .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email).IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username).IsUnique();

            modelBuilder.Entity<PasswordResetToken>()
                .HasIndex(t => new { t.UserId, t.TokenHash })
                .IsUnique();

            modelBuilder.Entity<PasswordResetToken>()
                .HasIndex(t => t.ExpiresAtUtc);

            modelBuilder.Entity<Project>()
                .Property(p => p.IsCompleted)
                .HasDefaultValue(false);
            }   
    }
}
