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
        public IEnumerable<ProjectTime> GetProjectTimes()
        {
            return _context.ProjectTimes.ToList();
        }

        public ProjectTime? GetById(int id)
        {
            return _context.ProjectTimes.FirstOrDefault(t => t.Id == id);
        }

        public IEnumerable<ProjectTime> GetByUserAndProjectId(int userId, int projectId)
        {
            return _context.ProjectTimes.Where(t => t.UserId == userId && t.ProjectId == projectId);
        }

        public void Add(ProjectTime projectTime)
        {
            _context.ProjectTimes.Add(projectTime);
        }
        
        public void Update(ProjectTime projectTime)
        {
            var existingProjectTime = _context.ProjectTimes.FirstOrDefault(p => p.Id == projectTime.Id);
            if (existingProjectTime != null)
            {
                existingProjectTime.StartTime = projectTime.StartTime;
                existingProjectTime.EndTime = projectTime.EndTime;
            }

        }

        public void Delete(int id)
        {
            var projectTime = GetById(id);
            if (projectTime != null)
            {
                _context.ProjectTimes.Remove(projectTime);
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
