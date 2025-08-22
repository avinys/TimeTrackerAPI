using TimeTrackerAPI.Models;
using TimeTrackerAPI.Repositories.Interfaces;
using TimeTrackerAPI.Services.Interfaces;

namespace TimeTrackerAPI.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _repo;
        public ProjectService(IProjectRepository repo)
        {
            _repo = repo;
        }
        public IEnumerable<Project> GetProjects()
        {
            return _repo.GetProjects();
        }
        public IEnumerable<Project> GetByUserId(int userId)
        {
            return _repo.GetByUserId(userId);
        }
        public Project? GetById(int id)
        {
            return _repo.GetById(id);
        }
        public Project Create(string name, int userId)
        {
            var proj = new Project
            {
                Name = name,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };
            _repo.Add(proj);
            _repo.Save();
            return proj;
        }
    }
}
