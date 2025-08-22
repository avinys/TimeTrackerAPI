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
        public IEnumerable<Project> GetProjects()
        {
            return _context.Projects.ToList();
        }
        public IEnumerable<Project> GetByUserId(int userId)
        {
            return _context.Projects.Where(p => p.UserId == userId);
        }
        public Project? GetById(int id)
        {
            return _context.Projects.FirstOrDefault(p => p.Id == id);
        }
        public void Add(Project project)
        {
            _context.Projects.Add(project);
        }
        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
