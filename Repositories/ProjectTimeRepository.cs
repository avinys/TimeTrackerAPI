using Microsoft.EntityFrameworkCore;
using TimeTrackerAPI.Data;
using TimeTrackerAPI.Models;
using TimeTrackerAPI.Repositories.Interfaces;

namespace TimeTrackerAPI.Repositories
{
    public class ProjectTimeRepository : IProjectTimeRepository
    {
        private readonly ApplicationDbContext _context;
        public ProjectTimeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<ProjectTime> Query() 
        {
           return _context.ProjectTimes;
        }

        public Task<ProjectTime?> GetByIdAsync(int id)
        {
            return _context.ProjectTimes.FirstOrDefaultAsync(pt => pt.Id == id);
        }

        public Task AddAsync(ProjectTime projectTime)
        {
            return _context.ProjectTimes.AddAsync(projectTime).AsTask();
        }

        public void Remove(ProjectTime projectTime)
        {
            _context.ProjectTimes.Remove(projectTime);
        }

        public Task SaveAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
