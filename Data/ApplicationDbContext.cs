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
    }
}
