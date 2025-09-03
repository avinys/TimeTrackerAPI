using Microsoft.EntityFrameworkCore;
using TimeTrackerAPI.Data;
using TimeTrackerAPI.Models;
using TimeTrackerAPI.Repositories.Interfaces;

namespace TimeTrackerAPI.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ApplicationDbContext _context;
        public ProjectRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public IQueryable<Project> Query()
        {
            return _context.Projects;
        }
        public  Task<Project?> GetByIdAsync(int id)
        {
            return _context.Projects.FirstOrDefaultAsync(p => p.Id == id);
        }
        public Task AddAsync(Project project)
        {
            return _context.Projects.AddAsync(project).AsTask();
        }
        public void Remove(Project project)
        {
            _context.Projects.Remove(project);
        }
        public Task SaveAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
